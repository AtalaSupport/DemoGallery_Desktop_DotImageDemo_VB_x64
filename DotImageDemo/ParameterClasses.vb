Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms
Imports Atalasoft.Imaging
Imports Atalasoft.Imaging.Codec
Imports Atalasoft.Imaging.Drawing
Imports Atalasoft.Imaging.WinControls

Namespace dotImageDemo

#Region "NewImageParameter"

    ''' <summary>
    ''' This class is used for the Property Grid when a new image is requested.
    ''' </summary>
    Public Class NewImageParameter
        Private width_Renamed As Integer = 400
        Private height_Renamed As Integer = 400
        Private backColor_Renamed As Color = Color.White
        Private imageFormat_Renamed As PixelFormat = PixelFormat.Pixel24bppBgr

        Public Sub New()
        End Sub

        'INSTANT VB NOTE: The parameter width was renamed since Visual Basic will not uniquely identify class members when parameters have the same name:
        'INSTANT VB NOTE: The parameter height was renamed since Visual Basic will not uniquely identify class members when parameters have the same name:
        'INSTANT VB NOTE: The parameter backColor was renamed since Visual Basic will not uniquely identify class members when parameters have the same name:
        'INSTANT VB NOTE: The parameter imageFormat was renamed since Visual Basic will not uniquely identify class members when parameters have the same name:
        Public Sub New(ByVal width_Renamed As Integer, ByVal height_Renamed As Integer, ByVal backColor_Renamed As Color, ByVal imageFormat_Renamed As PixelFormat)
            Me.Width = width_Renamed
            Me.Height = height_Renamed
            Me.BackColor = backColor_Renamed
            Me.imageFormat_Renamed = imageFormat_Renamed
        End Sub

        <Description("The width of the image in pixels."), DefaultValue(400)> _
        Public Property Width() As Integer
            Get
                Return width_Renamed
            End Get
            Set(ByVal Value As Integer)
                width_Renamed = Value
            End Set
        End Property

        <Description("The height of the image in pixels."), DefaultValue(400)> _
        Public Property Height() As Integer
            Get
                Return height_Renamed
            End Get
            Set(ByVal Value As Integer)
                height_Renamed = Value
            End Set
        End Property

        <Description("The background color of the image.")> _
        Public Property BackColor() As Color
            Get
                Return backColor_Renamed
            End Get
            Set(ByVal Value As Color)
                backColor_Renamed = Value
            End Set
        End Property

        <Description("The pixel format of the image."), DefaultValue(PixelFormat.Pixel24bppBgr)> _
        Public Property ImageFormat() As PixelFormat
            Get
                Return imageFormat_Renamed
            End Get
            Set(ByVal Value As PixelFormat)
                imageFormat_Renamed = Value
            End Set
        End Property
    End Class

    ''' <summary>
    ''' This class is used with the Property Grid when a URL is requested.
    ''' </summary>
    Public Class UrlParameter
        Private _url As String = ""
        Private _username As String = ""
        Private _password As String = ""
        Private _fileFormat As ImageFileFormats

        Public Sub New()
        End Sub

        <Description("The HTTP or FTP address of the image.")> _
        Public Property Url() As String
            Get
                Return _url
            End Get
            Set(ByVal Value As String)
                _url = Value
            End Set
        End Property

        Public Property Username() As String
            Get
                Return _username
            End Get
            Set(ByVal Value As String)
                _username = Value
            End Set
        End Property

        Public Property Password() As String
            Get
                Return _password
            End Get
            Set(ByVal Value As String)
                _password = Value
            End Set
        End Property

        Public Property FileFormat() As ImageFileFormats
            Get
                Return _fileFormat
            End Get
            Set(ByVal Value As ImageFileFormats)
                _fileFormat = Value
            End Set
        End Property

        Public Overrides Function ToString() As String
            If Not _url Is Nothing AndAlso _url.Length > 0 Then
                If _username.Length > 0 AndAlso _password.Length > 0 Then
                    Return "ftp://" & _username & ":" & _password & "@" & _url
                Else
                    Return _url
                End If
            Else
                Return ""
            End If
        End Function

    End Class

#End Region

#Region "Program Options"

    Public Class ProgramOptions
        Private asynchronous_Renamed As Boolean = True
        Private undoLevels_Renamed As Integer = 0
        Private antialias As AntialiasDisplayMode = AntialiasDisplayMode.None
        Private autoZoom_Renamed As AutoZoomMode = AutoZoomMode.None
        Private scrollbars As ScrollBarVisibility = ScrollBarVisibility.Dynamic

        Public Sub New()
        End Sub

        <Description("Runs the Workspace in asynchronous mode."), DefaultValue(True)> _
        Public Property Asynchronous() As Boolean
            Get
                Return asynchronous_Renamed
            End Get
            Set(ByVal Value As Boolean)
                asynchronous_Renamed = Value
            End Set
        End Property

        <Description("The number of undo levels to keep in memory."), DefaultValue(5)> _
        Public Property UndoLevels() As Integer
            Get
                Return undoLevels_Renamed
            End Get
            Set(ByVal Value As Integer)
                undoLevels_Renamed = Value
            End Set
        End Property

        <Description("The antialiasing mode, controling how the image is scaled at different zoom levels."), DefaultValue(AntialiasDisplayMode.None)> _
        Public Property AntialiasDisplay() As AntialiasDisplayMode
            Get
                Return antialias
            End Get
            Set(ByVal Value As AntialiasDisplayMode)
                antialias = Value
            End Set
        End Property

        <Description("A value indicating how the image should be zoomed as the control is resized."), DefaultValue(AutoZoomMode.None)> _
        Public Property AutoZoom() As AutoZoomMode
            Get
                Return autoZoom_Renamed
            End Get
            Set(ByVal Value As AutoZoomMode)
                autoZoom_Renamed = Value
            End Set
        End Property

        <Description("Sets the type of scrollbars to use, if any."), DefaultValue(ScrollBarVisibility.Dynamic)> _
        Public Property ScrollBarStyle() As ScrollBarVisibility
            Get
                Return scrollbars
            End Get
            Set(ByVal Value As ScrollBarVisibility)
                scrollbars = Value
            End Set
        End Property

    End Class

#End Region

    Public Enum FillMode
        Solid
        Hatch
    End Enum

    Public Enum ImageFileFormats
        Bmp
        Gif
        Jpeg
        Png

    End Enum

    Public Class DrawOutlineParameters : Inherits AtalaPen
        Private smoothingLevel_Renamed As Double = 0
        Public Sub New()
        End Sub

        Public Function GetPen() As AtalaPen
            Return CType(MyBase.Clone(), AtalaPen)
        End Function

        Public Property SmoothingLevel() As Double
            Get
                Return Me.smoothingLevel_Renamed
            End Get
            Set(ByVal Value As Double)
                Me.smoothingLevel_Renamed = Value
            End Set
        End Property
    End Class

    Public Class DrawSolidParameters : Inherits DrawOutlineParameters
        Private fillColor_Renamed As Color = Color.Transparent
        Private fillMode_Renamed As FillMode = FillMode.Solid
        Private hatch_Renamed As Hatch = Hatch.Cross

        Public Sub New()
        End Sub

        Public Property FillMode() As FillMode
            Get
                Return fillMode_Renamed
            End Get
            Set(ByVal Value As FillMode)
                fillMode_Renamed = Value
            End Set
        End Property

        Public Property Hatch() As Hatch
            Get
                Return hatch_Renamed
            End Get
            Set(ByVal Value As Hatch)
                hatch_Renamed = Value
            End Set
        End Property

        Public Property FillColor() As Color
            Get
                Return fillColor_Renamed
            End Get
            Set(ByVal Value As Color)
                fillColor_Renamed = Value
            End Set
        End Property

        Public Function GetFill() As Fill
            Select Case fillMode_Renamed
                Case FillMode.Solid
                    Return New SolidFill(fillColor_Renamed)
                Case FillMode.Hatch
                    Return New HatchedFill(hatch_Renamed, fillColor_Renamed)
                Case Else
                    Return Nothing
            End Select
        End Function
    End Class

#Region "Line Parameters"

    Public Class LineParameters : Inherits DrawOutlineParameters
        Private startPos As Point = New Point(10, 10)
        Private endPos As Point = New Point(100, 100)

        Public Sub New()
        End Sub

        Public Property StartPosition() As Point
            Get
                Return startPos
            End Get
            Set(ByVal Value As Point)
                startPos = Value
            End Set
        End Property

        Public Property EndPosition() As Point
            Get
                Return endPos
            End Get
            Set(ByVal Value As Point)
                endPos = Value
            End Set
        End Property

    End Class

#End Region

#Region "Lines Parameters"

    Public Class LinesParameters : Inherits DrawOutlineParameters
        Private points_Renamed As Point() = New Point(2) {New Point(10, 10), New Point(100, 250), New Point(10, 250)}

        Public Sub New()
        End Sub

        Public Property Points() As Point()
            Get
                Return points_Renamed
            End Get
            Set(ByVal Value As Point())
                points_Renamed = Value
            End Set
        End Property

    End Class

#End Region

#Region "Ellipse Parameters"

    Public Class EllipseParameters : Inherits DrawSolidParameters
        Private rectangle_Renamed As Rectangle = New Rectangle(10, 10, 100, 160)

        Public Sub New()
        End Sub

        Public Property Rectangle() As Rectangle
            Get
                Return rectangle_Renamed
            End Get
            Set(ByVal Value As Rectangle)
                rectangle_Renamed = Value
            End Set
        End Property

    End Class

#End Region

#Region "Rectangle Parameters"

    Public Class RectangleParameters : Inherits DrawSolidParameters
        Private rounding_Renamed As Size = New Size(0, 0)

        Public Sub New()
        End Sub

        Public Property Rounding() As Size
            Get
                Return rounding_Renamed
            End Get
            Set(ByVal Value As Size)
                rounding_Renamed = Value
            End Set
        End Property
    End Class

#End Region

#Region "FloodFill Parameters"

    Public Class FloodFillParameters
        Private position_Renamed As Point = New Point(50, 50)
        Private tolerance_Renamed As Integer = 0
        Private color_Renamed As Color = Color.Red
        Private surface As Boolean = False
        Private fillColor_Renamed As Color = Color.Transparent

        Public Sub New()
        End Sub

        Public Property Color() As Color
            Get
                Return color_Renamed
            End Get
            Set(ByVal Value As Color)
                color_Renamed = Value
            End Set
        End Property

        Public Property Position() As Point
            Get
                Return position_Renamed
            End Get
            Set(ByVal Value As Point)
                position_Renamed = Value
            End Set
        End Property

        Public Property Tolerance() As Integer
            Get
                Return tolerance_Renamed
            End Get
            Set(ByVal Value As Integer)
                tolerance_Renamed = Value
            End Set
        End Property

        Public Property SurfaceFill() As Boolean
            Get
                Return surface
            End Get
            Set(ByVal Value As Boolean)
                surface = Value
            End Set
        End Property

        Public Property FillColor() As Color
            Get
                Return fillColor_Renamed
            End Get
            Set(ByVal Value As Color)
                fillColor_Renamed = Value
            End Set
        End Property
    End Class

#End Region

#Region "Polygon Parameters"

    Public Class PolygonParameters : Inherits DrawSolidParameters
        Private points_Renamed As Point() = New Point(2) {New Point(10, 10), New Point(100, 250), New Point(10, 250)}

        Public Sub New()
        End Sub

        Public Property Points() As Point()
            Get
                Return points_Renamed
            End Get
            Set(ByVal Value As Point())
                points_Renamed = Value
            End Set
        End Property
    End Class

#End Region

#Region "Text Parameters"

    Public Class TextParameters : Inherits TextFormat
        Private text_Renamed As String = "Atalasoft dotImage"
        Private font_Renamed As Font = New Font("Verdana", 32)
        Private fillColor As Color = Color.Black
        Private smoothingLevel_Renamed As Double = 0
        Private position_Renamed As Point = Point.Empty
        Private _quality As FontQuality = FontQuality.Default

        Public Sub New()
        End Sub

        Public Property Text() As String
            Get
                Return text_Renamed
            End Get
            Set(ByVal Value As String)
                text_Renamed = Value
            End Set
        End Property

        Public Property Font() As Font
            Get
                Return font_Renamed
            End Get
            Set(ByVal Value As Font)
                font_Renamed = Value
            End Set
        End Property

        <Description("The color of the text.")> _
        Public Property Color() As Color
            Get
                Return fillColor
            End Get
            Set(ByVal Value As Color)
                fillColor = Value
            End Set
        End Property

        Public Property SmoothingLevel() As Double
            Get
                Return Me.smoothingLevel_Renamed
            End Get
            Set(ByVal Value As Double)
                Me.smoothingLevel_Renamed = Value
            End Set
        End Property

        Public Function GetTextFormat() As TextFormat
            Dim tf As TextFormat = New TextFormat(MyBase.Alignment, MyBase.Angle, MyBase.InterCharacterSpace)
            Return tf
        End Function

        <Description("The top/left position to place the text." & Constants.vbLf & "This is ignored if a selection is visible.")> _
        Public Property Position() As Point
            Get
                Return Me.position_Renamed
            End Get
            Set(ByVal Value As Point)
                Me.position_Renamed = Value
            End Set
        End Property

        Public Property FontQuality() As FontQuality
            Get
                Return Me._quality
            End Get
            Set(ByVal Value As FontQuality)
                Me._quality = Value
            End Set
        End Property

    End Class

#End Region

    Public Class PointParameter
        Private pt As Point = New Point(0, 0)

        Public Sub New()
        End Sub

        Public Property Position() As Point
            Get
                Return pt
            End Get
            Set(ByVal Value As Point)
                pt = Value
            End Set
        End Property
    End Class

    Public Class TlaPassword
        Private password_Renamed As String = ""

        Public Sub New()
        End Sub

        <Description("The password for this TLA file.  Leave black if there is no password.")> _
        Public Property Password() As String
            Get
                Return password_Renamed
            End Get
            Set(ByVal Value As String)
                password_Renamed = Value
            End Set
        End Property
    End Class

    Public Class ImageFileFormat
        Private saveType As ImageType = ImageType.Jpeg

        Public Sub New()
        End Sub

        Public Property ImageFormat() As ImageType
            Get
                Return Me.saveType
            End Get
            Set(ByVal Value As ImageType)
                Me.saveType = Value
            End Set
        End Property

    End Class

End Namespace
