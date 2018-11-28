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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//Librerías para multiprocesamiento
using System.Threading;
using System.Diagnostics;

namespace PracticaMovimiento
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Stopwatch stopwatch;
        TimeSpan tiempoAnterior;


        enum EstadoJuego { Gameplay, Gameover };
        EstadoJuego estadoActual = EstadoJuego.Gameplay;

        enum Direccion { Arriba, Abajo, Izquierda, Derecha, Ninguna };
        Direccion direccionJugador = Direccion.Ninguna;

        double velocidadRana = 80;

        public MainWindow()
        {
            
            InitializeComponent();

            stopwatch = new Stopwatch();


            tiempoAnterior = stopwatch.Elapsed;

            //1.- Establecer instrucciones
            ThreadStart threadStart =
                new ThreadStart(actualizar);
            //2.- Inicializar el Thread
            Thread threadMoverEnemigos =
                new Thread(threadStart);
            //3.- Ejecutar el Thread
            threadMoverEnemigos.Start();

            /*imgRana.RenderTransform =
                new RotateTransform(90);*/
            
        }

        void moverJugador(TimeSpan deltaTime)
        {
            double topRanaActual = Canvas.GetTop(imgRana);
            double leftRanaActual = Canvas.GetLeft(imgRana);
            switch (direccionJugador)
            {
                case Direccion.Arriba:
                    Canvas.SetTop(imgRana, 
                        topRanaActual - (velocidadRana * deltaTime.TotalSeconds));
                    break;
                case Direccion.Abajo:
                    Canvas.SetTop(imgRana,
                        topRanaActual + (velocidadRana * deltaTime.TotalSeconds));
                    break;
                case Direccion.Izquierda:
                    if (leftRanaActual - 
                        (velocidadRana * deltaTime.TotalSeconds) >= 0)
                    {
                        Canvas.SetLeft(imgRana,
                        leftRanaActual - 
                        (velocidadRana * deltaTime.TotalSeconds));
                    }
                    
                    break;
                case Direccion.Derecha:
                    double nuevaPosicion = leftRanaActual + 
                        (velocidadRana * deltaTime.TotalSeconds);
                    if (nuevaPosicion + imgRana.Width <= 800)
                    {
                        Canvas.SetLeft(imgRana,
                        nuevaPosicion);
                    }
                    
                    break;
                case Direccion.Ninguna:
                    break;
                default:
                    break;
            }
        }

        void actualizar()
        {


            while (true)
            {


                Dispatcher.Invoke(
                () =>
                {
                    var tiempoActual = stopwatch.Elapsed;
                    var deltaTime =
                        tiempoActual - tiempoAnterior;
                    double divisor = 500;
                    //velocidadRana += 10 * deltaTime.TotalSeconds;

                    if (estadoActual == EstadoJuego.Gameplay)
                    {
                        moverJugador(deltaTime);
                        double leftCarroActual =
                        Canvas.GetTop(imgCarro);
                        Canvas.SetTop(
                            imgCarro, leftCarroActual + (100 * deltaTime.TotalSeconds));
                        if (Canvas.GetTop(imgCarro) >= 480)
                        {
                            Canvas.SetTop(imgCarro, -100);
                        }

                        //Intersección en X
                        double xCarro =
                            Canvas.GetLeft(imgCarro);
                        double xRana =
                            Canvas.GetLeft(imgRana);
                        if (xRana + imgRana.Width >= xCarro &&
                            xRana <= xCarro + imgCarro.Width)
                        {
                            lblInterseccionX.Text =
                            "SI HAY INTERSECCION EN X!!!";
                        }
                        else
                        {
                            lblInterseccionX.Text =
                            "No hay interseccion en X";
                        }
                        double yCarro =
                            Canvas.GetTop(imgCarro);
                        double yRana =
                            Canvas.GetTop(imgRana);
                        if (yRana + imgRana.Height >= yCarro &&
                            yRana <= yCarro + imgCarro.Height)
                        {
                            lblInterseccionY.Text =
                                "SI HAY INTERSECCION EN Y!!!";
                        }
                        else
                        {
                            lblInterseccionY.Text =
                                "No hay interseccion en Y";
                        }

                        if (xRana + imgRana.Width >= xCarro &&
                            xRana <= xCarro + imgCarro.Width &&
                            yRana + imgRana.Height >= yCarro &&
                            yRana <= yCarro + imgCarro.Height)
                        {
                            lblColision.Text =
                                "HAY COLISION!!!";
                            estadoActual = EstadoJuego.Gameover;
                            miCanvas.Visibility = Visibility.Collapsed;
                            canvasGameOver.Visibility =
                                Visibility.Visible;
                        }
                        else
                        {
                            lblColision.Text =
                                "No hay colision";
                        }
                        
                        lblPuntos.Text = (stopwatch.ElapsedMilliseconds/divisor).ToString();

                    }
                    else if (estadoActual == EstadoJuego.Gameover)
                    {

                    }

                    

                    


                    tiempoAnterior = tiempoActual;
                   

                }
                );
            }
            
        }

        private void miCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (estadoActual == EstadoJuego.Gameplay)
            {
                if (e.Key == Key.Left)
                {
                    direccionJugador = Direccion.Izquierda;
                }
                if (e.Key == Key.Right)
                {
                    direccionJugador = Direccion.Derecha;
                }
            }
        }

        

        private void miCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left &&
                direccionJugador == Direccion.Izquierda)
            {
                direccionJugador = Direccion.Ninguna;
            }
            if (e.Key == Key.Right &&
                direccionJugador == Direccion.Derecha)
            {
                direccionJugador = Direccion.Ninguna;
            }
        }
        int naveSeleccion = 1;

        private void rbPlayer1_Checked(object sender, RoutedEventArgs e)
        {
            naveSeleccion = 1;
        }

        private void rbPlayer2_Checked(object sender, RoutedEventArgs e)
        {
            naveSeleccion = 2;

        }

        private void rbPlayer3_Checked(object sender, RoutedEventArgs e)
        {
            naveSeleccion = 3;

        }


        public void Start_Click(object sender, RoutedEventArgs e)
        {


            switch (naveSeleccion)
            {
                case 1:
                    BitmapImage nave1 = new BitmapImage();
                    nave1.BeginInit();
                    nave1.UriSource = new Uri(@"\Assets\navePlayer1.png", UriKind.Relative);
                    nave1.EndInit(); 
                    imgRana.Source = nave1;
                    break;
                case 2:
                    BitmapImage nave2 = new BitmapImage();
                    nave2.BeginInit();
                    nave2.UriSource = new Uri(@"\Assets\navePlayer2.png", UriKind.Relative);
                    nave2.EndInit();
                    imgRana.Source = nave2;
                    break;
                case 3:
                    BitmapImage nave3 = new BitmapImage();
                    nave3.BeginInit();
                    nave3.UriSource = new Uri(@"\Assets\navePlayer3.png", UriKind.Relative);
                    nave3.EndInit();
                    imgRana.Source = nave3;
                    break;
            }
            canvasInicio.Visibility = Visibility.Collapsed;
            miCanvas.Visibility = Visibility.Visible;
            stopwatch.Start();
            miCanvas.Focus();
        }

        
    }
}
