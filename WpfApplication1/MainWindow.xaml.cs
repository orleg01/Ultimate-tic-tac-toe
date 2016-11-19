using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication1.GameLogic;

namespace WpfApplication1
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Brush NORMAL = Brushes.LightGray;
        private Brush PLAY   = Brushes.LightGreen;
        private Brush WIN    = Brushes.Gold;
        private Brush LOSE   = Brushes.Red;

        private Manager.BoardStatuse[,] boardStatuse;
        private Manager manager = new Manager();
        private bool gameIsFinish = false;

        public MainWindow()
        {
            InitializeComponent();

            CreateBoard();
            
            paintAllBoard( );
            
        }









        #region    helper function   
        private Object getElementFromLayOut(Grid grid, int row, int col)
        {
            return grid.Children
                       .Cast<UIElement>()
                       .First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
        }
        #endregion

        #region paintFunction
        private void  paintBoardAsPlayable( int row , int col , Brush paint)
        {
            Grid grid = (Grid)getElementFromLayOut(Board, row, col);

            for (int i = 0; i < grid.Children.Count; i++)
            {
                Button but = (Button)grid.Children[i];
                but.Background = paint;

                /*
                if(paint == PLAY)
                    but.IsEnabled = true;
                else
                    but.IsEnabled = false;
                */

            }
        }
        private void paintAllBoard(bool yourTurn = true)
        {
            for (int i = 0; i < Board.Children.Count; i++)
            {
                Grid grid = (Grid)Board.Children[i];
                if (boardStatuse[i / 3, i % 3] == Manager.BoardStatuse.PROCCES)
                {
                    for (int j = 0; j < grid.Children.Count; j++)
                    {
                        Button but = (Button)grid.Children[j];
                        if (yourTurn)
                        {
                            but.Background = PLAY;
                           // but.IsEnabled = true;
                        }
                        else
                        {
                            but.Background =  NORMAL;
                           // but.IsEnabled = false;
                        }
                    }
                }
            }
        }
        #endregion

        #region board creation
        private void CreateBoard()
        {
            int rowOfPanel = Board.RowDefinitions.Count;
            int colOfPanel = Board.ColumnDefinitions.Count;

            boardStatuse = new Manager.BoardStatuse[3, 3];

            for (int i = 0; i < rowOfPanel; i++)
                for (int j = 0; j < colOfPanel; j++)
                {
                    Grid grid = getNewEmptyGrid(i , j);
                    Grid.SetRow(grid, i);
                    Grid.SetColumn(grid, j);
                    Board.Children.Add(grid);
                    boardStatuse[i, j] = Manager.BoardStatuse.PROCCES;
                }
            

        }
        private static readonly int GRID_MARGIN = 5;
        private Grid getNewEmptyGrid(int bigRow , int bigCol)
        {

            Grid grid = new Grid();

            grid.Margin = new Thickness(GRID_MARGIN);

            for (int i = 0; i < 3; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {

                    Button but = new Button();
                    but.Click += ClickOnButton;

                    Style newStyle = new Style();
                    Setter newSetter = new Setter(Label.FontSizeProperty, 30.0);
                    newStyle.Setters.Add(newSetter);
                    but.Style = newStyle;

                    but.Content = "-";
                    but.Background = NORMAL;
                    Grid.SetRow(but, i);
                    Grid.SetColumn(but, j);
                    but.Tag = new PlaceInformation( bigRow , i , bigCol , j);

                    grid.Children.Add(but);
                }

            return grid;
        }

        private void ClickOnButton(object sender, RoutedEventArgs e)
        {

            if (!gameIsFinish)
            {
                Button but = (Button)sender;

                int col, row;

                Manager.BoardStatuse moveState = manager.makeMove((PlaceInformation)but.Tag , out row , out col);
                if (moveState != Manager.BoardStatuse.FAILED)
                {
                    but.Content = "X";
                    //but.IsEnabled = false;

                    if (moveState == Manager.BoardStatuse.LOSE)
                    {
                        paintBoardAsPlayable(((PlaceInformation)but.Tag).BigRow, ((PlaceInformation)but.Tag).BigCol, LOSE);
                        boardStatuse[((PlaceInformation)but.Tag).BigRow, ((PlaceInformation)but.Tag).BigCol] = Manager.BoardStatuse.LOSE;
                    }
                    else if (moveState == Manager.BoardStatuse.WIN)
                    {
                        paintBoardAsPlayable(((PlaceInformation)but.Tag).BigRow, ((PlaceInformation)but.Tag).BigCol, WIN);
                        boardStatuse[((PlaceInformation)but.Tag).BigRow, ((PlaceInformation)but.Tag).BigCol] = Manager.BoardStatuse.WIN;
                    }
                    else if (moveState == Manager.BoardStatuse.ABSULOT_WIN)
                    {
                        boardStatuse[((PlaceInformation)but.Tag).BigRow, ((PlaceInformation)but.Tag).BigCol] = Manager.BoardStatuse.WIN;
                        paintBoardAsPlayable(((PlaceInformation)but.Tag).BigRow, ((PlaceInformation)but.Tag).BigCol, WIN);
                        gameIsFinish = true;
                    }
                    else if (moveState == Manager.BoardStatuse.ABSULOT_LOSE)
                    {
                        boardStatuse[((PlaceInformation)but.Tag).BigRow, ((PlaceInformation)but.Tag).BigCol] = Manager.BoardStatuse.LOSE;
                        paintBoardAsPlayable(((PlaceInformation)but.Tag).BigRow, ((PlaceInformation)but.Tag).BigCol, LOSE);
                        gameIsFinish = true;
                    }

                    paintAllBoard(false);
                    paintBoardAsPlayable(row, col, PLAY);
                }
            }

        }
        #endregion


    }
}
