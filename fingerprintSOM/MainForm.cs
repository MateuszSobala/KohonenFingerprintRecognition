// Kohonen SOM 2D Organizing
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using AForge.Neuro;
using AForge.Neuro.Learning;
using PatternRecognition.FingerprintRecognition.Core;
using PatternRecognition.FingerprintRecognition.FeatureExtractors;
using _2DOrganizing;

namespace SOMOrganizing
{
	/// <summary>
    /// Summary description for MainForm.
	/// </summary>
	public class MainForm : Form
    {
        private Button generateButton;
        private GroupBox groupBox2;
        private BufferedPanel mapPanel;
        private GroupBox groupBox3;
		private Label label3;
		private TextBox radiusBox;
		private Label label4;
		private TextBox rateBox;
		private Label label5;
		private TextBox iterationsBox;
		private Label label6;
		private Label label7;
		private TextBox currentIterationBox;
		private Label label8;
		private Button stopButton;
        private Button startButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;


		private const int	groupRadius = 20;
		private int         pointsCount = 100;
		private int[,]		points;// = new int[pointsCount, 2];	// x, y
		private double[][]	queryFeaturesSet;// = new double[pointsCount][];
		private int[,,]		map;
        private Bitmap inputImage;

	    private int[]       networkSize;
		private int			iterations		= 10;
		private double		learningRate	= 0.3;
		private int			learningRadius	= 3;
        private double angleMultiplier = 1;
        private double typeMultiplier = 1;
        private double coordsMultiplier = 1;
        private double hashCodeMultiplier = 0.0000000001;

		private Random		rand = new Random( );
		private Thread		workerThread;
        private TextBox textBox1;
        private Label label1;
        private Button button1;
        private TextBox textBoxCoordsMultiplier;
        private Label CoordsMultiplier;
        private TextBox textBoxTypeMultiplier;
        private Label TypeMultiplier;
        private TextBox textBoxAngleMultiplier;
        private Label AngleMultiplier;
        private Label label11;
        private TextBox textBoxHashCodeMultiplier;
        private Label label2;
        private Label label9;
        private TextBox textBoxQueryPerson;
        private volatile bool needToStop;

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent( );

			//
			GeneratePoints( );
			UpdateSettings( );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if ( components != null ) 
				{
					components.Dispose( );
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.generateButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mapPanel = new SOMOrganizing.BufferedPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxHashCodeMultiplier = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCoordsMultiplier = new System.Windows.Forms.TextBox();
            this.CoordsMultiplier = new System.Windows.Forms.Label();
            this.textBoxTypeMultiplier = new System.Windows.Forms.TextBox();
            this.TypeMultiplier = new System.Windows.Forms.Label();
            this.textBoxAngleMultiplier = new System.Windows.Forms.TextBox();
            this.AngleMultiplier = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.currentIterationBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.radiusBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rateBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.iterationsBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxQueryPerson = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(590, 53);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(75, 23);
            this.generateButton.TabIndex = 1;
            this.generateButton.Text = "Clear";
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mapPanel);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(448, 488);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input finger";
            // 
            // mapPanel
            // 
            this.mapPanel.BackColor = System.Drawing.Color.White;
            this.mapPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapPanel.Location = new System.Drawing.Point(6, 20);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(431, 460);
            this.mapPanel.TabIndex = 0;
            this.mapPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mapPanel_Paint);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxHashCodeMultiplier);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.textBoxCoordsMultiplier);
            this.groupBox3.Controls.Add(this.CoordsMultiplier);
            this.groupBox3.Controls.Add(this.textBoxTypeMultiplier);
            this.groupBox3.Controls.Add(this.TypeMultiplier);
            this.groupBox3.Controls.Add(this.textBoxAngleMultiplier);
            this.groupBox3.Controls.Add(this.AngleMultiplier);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.stopButton);
            this.groupBox3.Controls.Add(this.startButton);
            this.groupBox3.Controls.Add(this.currentIterationBox);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.radiusBox);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.rateBox);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.iterationsBox);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(483, 113);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(180, 387);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Neural Network";
            // 
            // textBoxHashCodeMultiplier
            // 
            this.textBoxHashCodeMultiplier.Location = new System.Drawing.Point(107, 126);
            this.textBoxHashCodeMultiplier.Name = "textBoxHashCodeMultiplier";
            this.textBoxHashCodeMultiplier.Size = new System.Drawing.Size(60, 20);
            this.textBoxHashCodeMultiplier.TabIndex = 25;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 34);
            this.label2.TabIndex = 24;
            this.label2.Text = "HashCode multiplier:";
            // 
            // textBoxCoordsMultiplier
            // 
            this.textBoxCoordsMultiplier.Location = new System.Drawing.Point(107, 93);
            this.textBoxCoordsMultiplier.Name = "textBoxCoordsMultiplier";
            this.textBoxCoordsMultiplier.Size = new System.Drawing.Size(60, 20);
            this.textBoxCoordsMultiplier.TabIndex = 23;
            // 
            // CoordsMultiplier
            // 
            this.CoordsMultiplier.Location = new System.Drawing.Point(7, 95);
            this.CoordsMultiplier.Name = "CoordsMultiplier";
            this.CoordsMultiplier.Size = new System.Drawing.Size(100, 16);
            this.CoordsMultiplier.TabIndex = 22;
            this.CoordsMultiplier.Text = "Coords multiplier:";
            // 
            // textBoxTypeMultiplier
            // 
            this.textBoxTypeMultiplier.Location = new System.Drawing.Point(107, 68);
            this.textBoxTypeMultiplier.Name = "textBoxTypeMultiplier";
            this.textBoxTypeMultiplier.Size = new System.Drawing.Size(60, 20);
            this.textBoxTypeMultiplier.TabIndex = 21;
            // 
            // TypeMultiplier
            // 
            this.TypeMultiplier.Location = new System.Drawing.Point(7, 70);
            this.TypeMultiplier.Name = "TypeMultiplier";
            this.TypeMultiplier.Size = new System.Drawing.Size(100, 16);
            this.TypeMultiplier.TabIndex = 20;
            this.TypeMultiplier.Text = "Type multiplier:";
            // 
            // textBoxAngleMultiplier
            // 
            this.textBoxAngleMultiplier.Location = new System.Drawing.Point(107, 43);
            this.textBoxAngleMultiplier.Name = "textBoxAngleMultiplier";
            this.textBoxAngleMultiplier.Size = new System.Drawing.Size(60, 20);
            this.textBoxAngleMultiplier.TabIndex = 19;
            // 
            // AngleMultiplier
            // 
            this.AngleMultiplier.Location = new System.Drawing.Point(7, 45);
            this.AngleMultiplier.Name = "AngleMultiplier";
            this.AngleMultiplier.Size = new System.Drawing.Size(94, 16);
            this.AngleMultiplier.TabIndex = 18;
            this.AngleMultiplier.Text = "Angle multiplier:";
            this.AngleMultiplier.Click += new System.EventHandler(this.label10_Click);
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Location = new System.Drawing.Point(7, 33);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(160, 2);
            this.label11.TabIndex = 17;
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(95, 292);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 16;
            this.stopButton.Text = "S&top";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(10, 292);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 15;
            this.startButton.Text = "&Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // currentIterationBox
            // 
            this.currentIterationBox.Location = new System.Drawing.Point(107, 254);
            this.currentIterationBox.Name = "currentIterationBox";
            this.currentIterationBox.ReadOnly = true;
            this.currentIterationBox.Size = new System.Drawing.Size(60, 20);
            this.currentIterationBox.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(7, 256);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 16);
            this.label8.TabIndex = 13;
            this.label8.Text = "Current iteration:";
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(7, 242);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(160, 2);
            this.label7.TabIndex = 12;
            // 
            // radiusBox
            // 
            this.radiusBox.Location = new System.Drawing.Point(107, 214);
            this.radiusBox.Name = "radiusBox";
            this.radiusBox.Size = new System.Drawing.Size(60, 20);
            this.radiusBox.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(7, 216);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 16);
            this.label4.TabIndex = 10;
            this.label4.Text = "Initial radius:";
            // 
            // rateBox
            // 
            this.rateBox.Location = new System.Drawing.Point(107, 189);
            this.rateBox.Name = "rateBox";
            this.rateBox.Size = new System.Drawing.Size(60, 20);
            this.rateBox.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(7, 191);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Initial learning rate:";
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point(107, 164);
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size(60, 20);
            this.iterationsBox.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(7, 166);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 16);
            this.label6.TabIndex = 6;
            this.label6.Text = "Iterations:";
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(7, 154);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(160, 2);
            this.label3.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(212, 542);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(106, 545);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 17;
            this.label1.Text = "Matched person:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(483, 53);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Load image";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(106, 514);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 16);
            this.label9.TabIndex = 20;
            this.label9.Text = "Query person:";
            // 
            // textBoxQueryPerson
            // 
            this.textBoxQueryPerson.Location = new System.Drawing.Point(212, 511);
            this.textBoxQueryPerson.Name = "textBoxQueryPerson";
            this.textBoxQueryPerson.ReadOnly = true;
            this.textBoxQueryPerson.Size = new System.Drawing.Size(100, 20);
            this.textBoxQueryPerson.TabIndex = 19;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(684, 599);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxQueryPerson);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Kohonen SOM 2D Organizing";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main( ) 
		{
			Application.Run( new MainForm( ) );
		}

        // Delegates to enable async calls for setting controls properties
        private delegate void SetTextCallback( Control control, string text );

        // Thread safe updating of control's text property
        private void SetText( Control control, string text )
        {
            if ( control.InvokeRequired )
            {
                SetTextCallback d = SetText;
                Invoke( d, control, text);
            }
            else
            {
                control.Text = text;
            }
        }
        
        // On main form closing
		private void MainForm_Closing(object sender, CancelEventArgs e)
		{
			// check if worker thread is running
			if ( ( workerThread != null ) && ( workerThread.IsAlive ) )
			{
				needToStop = true;
                while ( !workerThread.Join( 100 ) )
                    Application.DoEvents( );
            }
		}

		// Update settings controls
		private void UpdateSettings( )
		{
			iterationsBox.Text	= iterations.ToString( );
			rateBox.Text		= learningRate.ToString( );
			radiusBox.Text		= learningRadius.ToString( );
		    textBoxCoordsMultiplier.Text = coordsMultiplier.ToString();
            textBoxAngleMultiplier.Text = angleMultiplier.ToString();
            textBoxHashCodeMultiplier.Text = hashCodeMultiplier.ToString();
            textBoxTypeMultiplier.Text = typeMultiplier.ToString();
		}

		// On "Generate" button click
		private void generateButton_Click(object sender, EventArgs e)
		{
			GeneratePoints( );
		}

	    private MinutieResource _resource;

		// Generate point
		private void GeneratePoints( )
		{
			map = null;
		    mapPanel.BackgroundImage = null;
			mapPanel.Invalidate( );
		}

        //private int matchCount1;

		// Paint points

		private void pointsPanel_Paint( object sender, PaintEventArgs e )
		{
		    if (points == null)
		        return; 

			Graphics g = e.Graphics;

			using ( Brush brush = new SolidBrush( Color.Red ) )
			{
				// draw all points
                for (int i = 0, n = points.GetLength(0); i < n; i++)
				{
					g.FillEllipse( brush, points[i, 0], points[i, 1], 10, 10 );
				}
			}

            if (map != null && needToStop)
		    {
                using (Brush brush = new SolidBrush(Color.Magenta))
                {
                    // draw all points
                    for (int i = 0, n = map.GetLength(0); i < n; i++)
                    {
                        for (int j = 0, k = map.GetLength(1); j < k; j++)
                        {
                            if (map[i, j, 2] == 0)
                                continue;

                            g.FillEllipse(brush, map[i, j, 0], map[i, j, 1], 10, 10);   
                        }
                    }
                }
		    }
		}

		// Paint map
		private void mapPanel_Paint( object sender, PaintEventArgs e )
		{
			Graphics g = e.Graphics;

			if ( map != null )
			{
				// pens and brushes
                Brush brush = new SolidBrush(Color.SpringGreen);

				// lock
				Monitor.Enter( this );

				// draw the map
				for ( int i = 0, n = map.GetLength( 0 ); i < n; i++ )
				{
					for ( int j = 0, k = map.GetLength( 1 ); j < k; j++ )
					{
						if ( map[i, j, 2] == 0 )
							continue;

						// draw the point
						g.FillEllipse(brush, map[i, j, 0], map[i, j, 1], 5, 5 );
					}
				}

				// unlock
				Monitor.Exit( this );

				brush.Dispose( );
			}
		}

        // Delegates to enable async calls for setting controls properties
        private delegate void EnableCallback( bool enable );

        // Enable/disale controls (safe for threading)
        private void EnableControls( bool enable )
		{
            if ( InvokeRequired )
            {
                EnableCallback d = EnableControls;
                Invoke( d, enable);
            }
            else
            {
                iterationsBox.Enabled   = enable;
                rateBox.Enabled         = enable;
                radiusBox.Enabled       = enable;

                startButton.Enabled     = enable;
                generateButton.Enabled  = enable;
                stopButton.Enabled      = !enable;
            }
		}

	    private SOMParams _SOMParams;

		// On "Start" button click
		private void startButton_Click(object sender, EventArgs e)
		{
			// get iterations count
			try
			{
				iterations = Math.Max(5, Math.Min(1000000, int.Parse( iterationsBox.Text )));
			}
			catch
			{
				iterations = 500;
			}
			// get learning rate
			try
			{
                learningRate = Math.Max(0.00001, Math.Min(1.0, double.Parse( rateBox.Text )));
			}
			catch
			{
				learningRate = 0.3;
			}
			// get radius
			try
			{
				learningRadius = Math.Max(1, Math.Min(30, int.Parse( radiusBox.Text )));
			}
			catch
			{
				learningRadius = 3;
			}

            try
            {
                angleMultiplier = Math.Max(0.00001, Math.Min(100, double.Parse(textBoxAngleMultiplier.Text)));
            }
            catch
            {
                angleMultiplier = 1;
            }

            try
            {
                typeMultiplier = Math.Max(0.00001, Math.Min(100, double.Parse(textBoxTypeMultiplier.Text)));
            }
            catch
            {
                typeMultiplier = 1;
            }

            try
            {
                coordsMultiplier = Math.Max(0.00001, Math.Min(100, double.Parse(textBoxCoordsMultiplier.Text)));
            }
            catch
            {
                coordsMultiplier = 1;
            }

            try
            {
                hashCodeMultiplier = Math.Max(0, Math.Min(100, double.Parse(textBoxHashCodeMultiplier.Text)));
            }
            catch
            {
                hashCodeMultiplier = 1;
            }
			// update settings controls
			UpdateSettings( );

            _SOMParams = new SOMParams(learningRadius, learningRate, angleMultiplier, typeMultiplier, coordsMultiplier, hashCodeMultiplier, iterations, 5);

            // generate training set
            queryFeaturesSet = NetworkHelper.GetQueryFeaturesSet(inputImage, _SOMParams);

			// disable all settings controls except "Stop" button
			EnableControls( false );

			// run worker thread
			needToStop = false;
			workerThread = new Thread( SearchSolution );
			workerThread.Start( );
		}

		// On "Stop" button click
		private void stopButton_Click(object sender, EventArgs e)
		{
			// stop worker thread
			needToStop = true;
            while ( !workerThread.Join( 100 ) )
                Application.DoEvents( );
            workerThread = null;
		}

		// Worker thread
		private void SearchSolution( )
		{
            _resource = new MinutieResource();
            NetworkHelper networkHelper;
		    if (MinutieResource.IsAlreadyTaughtAndSaved(_SOMParams))
		    {
                networkHelper = new NetworkHelper(_resource.LoadTaughtPeople(_SOMParams), _SOMParams);
                networkHelper.Teach(iterations, iterations-1);
                SetText(currentIterationBox, _SOMParams.Iterations.ToString());
		    }
		    else
		    {
                networkHelper = new NetworkHelper(_resource.GetAllFeatures(), _SOMParams);

                var iter = 0;
                while (!needToStop)
                {
                    networkHelper.Teach(iterations, iter);

                    iter++;

                    SetText(currentIterationBox, iter.ToString());

                    if (iter >= iterations-1)
                        needToStop = true;
                }

                //_resource.SaveTaughtPeople(networkHelper._inputs, _SOMParams);

                networkHelper.Teach(iterations, iter);
		        iter++;
                SetText(currentIterationBox, iter.ToString());
		    }

            // create map
            map = new int[networkHelper._networkSize[0], networkHelper._networkSize[1], 3];

            UpdateMap(networkHelper);
            FindPerson();

			// enable settings controls
			EnableControls( true );            
		}

	    private List<int> _winners;

		// Update map
        private void UpdateMap(NetworkHelper network)
		{
			// get first layer
			var layer = network._som.Layers[0];

			// lock
			Monitor.Enter( this );

			// run through all neurons
			for ( var i = 0; i < layer.Neurons.Length; i++ )
			{
				var neuron = layer.Neurons[i];

                var x = i % network._networkSize[0];
                var y = i / network._networkSize[0];

				map[x, y, 0] = (int) neuron.Weights[2];
                map[x, y, 1] = (int) neuron.Weights[3];
				map[x, y, 2] = 0;
			}

            _winners = new List<int>();
			// collect active neurons
            foreach (var queryFeature in queryFeaturesSet)
            {
                network._som.Compute(queryFeature);
                var w = network._som.GetWinner();

                map[w % network._networkSize[0], w / network._networkSize[0], 2] = 1;

                _winners.Add(w);
            }

			// unlock
			Monitor.Exit( this );

			//
			mapPanel.Invalidate( );
		}

        private void FindPerson()
        {
            SetText(textBox1, _resource.GetWinningPerson(_winners));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.bmp, *.jpg, *.tif)|*.bmp;*.jpg;*.tif",
                FilterIndex = 1,
                Multiselect = false
            };

            var dialogResult = dialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                inputImage = ImageLoader.LoadImage(dialog.FileName);
                SetText(textBoxQueryPerson, dialog.SafeFileName.Substring(0,3));
                mapPanel.BackgroundImage = inputImage;
                mapPanel.Invalidate();
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
	}
}
