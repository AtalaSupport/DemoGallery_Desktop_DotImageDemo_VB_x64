Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms
Imports Atalasoft.Imaging.ImageProcessing
Imports Atalasoft.Imaging.Drawing
Imports Atalasoft.Imaging

Namespace dotImageDemo
	''' <summary>
	''' Summary description for Histogram.
	''' </summary>
	Public Class Histogram
		Inherits System.Windows.Forms.Form
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.Container = Nothing
		Private hist As Integer()

		Public Sub New()
			'
			' Required for Windows Form Designer support
			'
			InitializeComponent()
			MyBase.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.DoubleBuffer Or ControlStyles.UserPaint, True)

			'
			' TODO: Add any constructor code after InitializeComponent call
			'
		End Sub

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				If Not components Is Nothing Then
					components.Dispose()
				End If
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Windows Form Designer generated code"
		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			' 
			' Histogram
			' 
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.BackColor = System.Drawing.Color.White
			Me.ClientSize = New System.Drawing.Size(248, 66)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
			Me.Name = "Histogram"
			Me.Text = "Histogram"
'			Me.Layout += New System.Windows.Forms.LayoutEventHandler(Me.Histogram_Layout);
'			Me.Paint += New System.Windows.Forms.PaintEventHandler(Me.Histogram_Paint);

		End Sub
		#End Region

		Public Sub SetHistogram(ByVal image As AtalaImage)
			Dim getHist As Atalasoft.Imaging.ImageProcessing.Histogram = New Atalasoft.Imaging.ImageProcessing.Histogram(image)

			Select Case image.PixelFormat
				Case PixelFormat.Pixel1bppIndexed
					If AtalaImage.Edition = LicenseEdition.Document Then
						Me.hist = getHist.GetDocumentHistogram()
					Else
						Me.hist = New Integer(255){}
					End If
				Case PixelFormat.Pixel8bppGrayscale, PixelFormat.Pixel16bppGrayscaleAlpha, PixelFormat.Pixel16bppGrayscale
					Me.hist = getHist.GetChannelHistogram(0)
				Case PixelFormat.Pixel24bppBgr, PixelFormat.Pixel32bppBgr, PixelFormat.Pixel32bppBgra, PixelFormat.Pixel48bppBgr, PixelFormat.Pixel64bppBgra
					Me.hist = getHist.GetBrightnessHistogram()
				Case Else 'incompatible
					Me.hist = New Integer(255){}
			End Select

			'draw histogram
			Me.Invalidate()

		End Sub

		Private Sub Histogram_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
			If Not hist Is Nothing Then
				'get max value in histogram
				Dim max As Integer = 0
				For Each val As Integer In hist
					If val > max Then
						max = val
					End If
				Next val

				' This will happen if the pixel format is not compatible.
				If max = 0 Then
				max = 1
				End If

				'clear display
				e.Graphics.FillRectangle(New SolidBrush(Color.White), Me.ClientRectangle)

				Dim w As Integer = Me.Width / hist.Length
				If w = 0 Then
					w = 1
				End If

				'draw histogram
				For i As Integer = 0 To hist.Length - 1
					e.Graphics.DrawLine(New Pen(Color.Black, w), New Point(i * w, Me.ClientSize.Height), New Point(i * w, Me.ClientSize.Height - (CInt(Fix((CDbl(hist(i)) / max) * Me.ClientSize.Height)))))
				Next i
			End If
		End Sub

		Private Sub Histogram_Layout(ByVal sender As Object, ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
			Me.Invalidate()
		End Sub
	End Class

	Public Enum HistogramType
		Brightness
		Channel
		Document
	End Enum

	Public Class HistogramOptions
		Private type_Renamed As HistogramType = HistogramType.Brightness
		Private channel_Renamed As Atalasoft.Imaging.ImageProcessing.ChannelFlags = Atalasoft.Imaging.ImageProcessing.ChannelFlags.Channel1

		Public Sub New()

		End Sub

		Public Property Type() As HistogramType
			Get
				Return Me.type_Renamed
			End Get
			Set
				Me.type_Renamed = Value
			End Set
		End Property

		Public Property Channel() As Atalasoft.Imaging.ImageProcessing.ChannelFlags
			Get
				Return Me.channel_Renamed
			End Get
			Set
				Me.channel_Renamed = Value
			End Set
		End Property

	End Class


End Namespace
