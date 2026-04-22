Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Data
Imports System.Threading
Imports Atalasoft.Imaging
Imports Atalasoft.Imaging.Drawing
Imports Atalasoft.Imaging.Codec
Imports Atalasoft.Imaging.Codec.CadCam
Imports Atalasoft.Imaging.Codec.Pdf
Imports Atalasoft.Imaging.Metadata
Imports Atalasoft.Imaging.ImageProcessing
Imports Atalasoft.Imaging.WinControls
Imports Atalasoft.Imaging.ImageProcessing.Channels
Imports Atalasoft.Imaging.ImageProcessing.Transforms
Imports Atalasoft.Imaging.ImageProcessing.Document
Imports Atalasoft.Imaging.ImageProcessing.Effects
Imports Atalasoft.Imaging.ImageProcessing.Filters
Imports Atalasoft.Imaging.ImageProcessing.Fft
#If JBIG2 Then
Imports Atalasoft.Imaging.Codec.Jbig2
Imports WinDemoHelperMethods.WinDemoHelperMethods

#End If
#If JPEG2000 Then
Imports Atalasoft.Imaging.Codec.Jpeg2000
#End If
Imports System.IO


Namespace dotImageDemo
    ''' <summary>
    ''' Main form for the Atalasoft dotImage Demo.
    ''' </summary>
    Public Class FormMain : Inherits System.Windows.Forms.Form
#Region "Private Vars"
        ' Image Information.
        Private _tempFiles As ArrayList = New ArrayList
        Private _images As ArrayList = New ArrayList
        Private _currentIndex As Integer
        Private _firstLoad As Boolean = True
        Private currentFile As String = ""
        Private jpegMarkers As JpegMarkerCollection = Nothing
        Private iptcItems As IptcCollection = Nothing
        Private exifItems As ExifCollection = Nothing
        Private comItems As ComTextCollection = Nothing
        Private transformChain As TransformChainCommand = Nothing
        Private tbSelectRectangle As System.Windows.Forms.ToolBarButton
        Private tbSelectEllipse As System.Windows.Forms.ToolBarButton
        Private histogram As Histogram = Nothing
        Private chainTransforms As Boolean = False
        Private performingOpen As Boolean = False
        Private isProcessError As Boolean

        Private Enum DrawMenuMode
            None = 0
            Line = 1
            Lines = 2
            Ellipse = 3
            Rectangle = 4
            FloodFill = 5
            Freehand = 6
            Polygon = 7
            Text = 8
        End Enum
#End Region

#Region "Designer Vars"
        Private components As System.ComponentModel.IContainer
        Private statusBarPosition As System.Windows.Forms.StatusBarPanel
        Private statusBarMessage As System.Windows.Forms.StatusBarPanel
        Private statusInfo As System.Windows.Forms.StatusBar
        Private WithEvents toolBar1 As System.Windows.Forms.ToolBar
        Private imageList1 As System.Windows.Forms.ImageList
        Private tbArrow As System.Windows.Forms.ToolBarButton
        Private tbPan As System.Windows.Forms.ToolBarButton
        Private statusBarProgress As System.Windows.Forms.StatusBarPanel
        Private progressBar1 As System.Windows.Forms.ProgressBar
        Private tbOpen As System.Windows.Forms.ToolBarButton
        Private tbSave As System.Windows.Forms.ToolBarButton
        Private tbSep As System.Windows.Forms.ToolBarButton
        Private tbUndo As System.Windows.Forms.ToolBarButton
        Private toolBarButton1 As System.Windows.Forms.ToolBarButton
        Private tbMagnifier As System.Windows.Forms.ToolBarButton
        Private tbZoom As System.Windows.Forms.ToolBarButton
        Private tbZoomSelection As System.Windows.Forms.ToolBarButton
        Private tbRedo As System.Windows.Forms.ToolBarButton

        ' Various State Data
        Private is_Disposed As Boolean = False
        Private currentTextBackColor As Color = Color.Transparent
        Private pageSetupDialog1 As System.Windows.Forms.PageSetupDialog
        Private printDialog1 As System.Windows.Forms.PrintDialog
        Private imagePrintDocument1 As Atalasoft.Imaging.WinControls.ImagePrintDocument
        Private ellipseRubberband As Atalasoft.Imaging.WinControls.EllipseRubberband
        Private rectangleSelection As Atalasoft.Imaging.WinControls.RectangleSelection
        Private WithEvents Viewer As Atalasoft.Imaging.WinControls.WorkspaceViewer
        Private WithEvents rectangleDraw As Atalasoft.Imaging.WinControls.RectangleRubberband
        Private WithEvents ellipseDraw As Atalasoft.Imaging.WinControls.EllipseRubberband
        Private WithEvents lineDraw As Atalasoft.Imaging.WinControls.LineRubberband
        Private drawMode As DrawMenuMode = DrawMenuMode.None
        Private canvasSmoothing As Double = 0
        Private mainMenu As System.Windows.Forms.MainMenu
        Private menuFile As System.Windows.Forms.MenuItem
        Private WithEvents menuFileNew As System.Windows.Forms.MenuItem
        Private WithEvents menuFileOpen As System.Windows.Forms.MenuItem
        Private WithEvents menuFileOpenFromURL As System.Windows.Forms.MenuItem
        Private menuItem1 As System.Windows.Forms.MenuItem
        Private WithEvents menuFileNoise As System.Windows.Forms.MenuItem
        Private menuItem2 As System.Windows.Forms.MenuItem
        Private WithEvents menuFileSaveAs As System.Windows.Forms.MenuItem
        Private WithEvents menuFileSaveFTP As System.Windows.Forms.MenuItem
        Private WithEvents menuFilePageSetup As System.Windows.Forms.MenuItem
        Private WithEvents menuFilePrintImage As System.Windows.Forms.MenuItem
        Private menuItem6 As System.Windows.Forms.MenuItem
        Private WithEvents menuFileExit As System.Windows.Forms.MenuItem
        Private WithEvents menuEditUndo As System.Windows.Forms.MenuItem
        Private WithEvents menuEditRedo As System.Windows.Forms.MenuItem
        Private menuItem8 As System.Windows.Forms.MenuItem
        Private WithEvents menuEditCut As System.Windows.Forms.MenuItem
        Private WithEvents menuEditCopy As System.Windows.Forms.MenuItem
        Private WithEvents menuEditPaste As System.Windows.Forms.MenuItem
        Private menuItem12 As System.Windows.Forms.MenuItem
        Private WithEvents menuEditOptions As System.Windows.Forms.MenuItem
        Private WithEvents menuImageInformation As System.Windows.Forms.MenuItem
        Private WithEvents menuImageChangePixelFormat As System.Windows.Forms.MenuItem
        Private WithEvents menuImageShowHistogram As System.Windows.Forms.MenuItem
        Private menuItem11 As System.Windows.Forms.MenuItem
        Private WithEvents menuImageExif As System.Windows.Forms.MenuItem
        Private menuImageZoom As System.Windows.Forms.MenuItem
        Private menuItem7 As System.Windows.Forms.MenuItem
        Private WithEvents menuImageIptc As System.Windows.Forms.MenuItem
        Private WithEvents menuImageZoom25 As System.Windows.Forms.MenuItem
        Private WithEvents menuImageZoom50 As System.Windows.Forms.MenuItem
        Private WithEvents menuImageZoom100 As System.Windows.Forms.MenuItem
        Private WithEvents menuImageZoom200 As System.Windows.Forms.MenuItem
        Private WithEvents menuImageZoom500 As System.Windows.Forms.MenuItem
        Private WithEvents menuImageZoom1000 As System.Windows.Forms.MenuItem
        Private menuItem9 As System.Windows.Forms.MenuItem
        Private menuEdit As System.Windows.Forms.MenuItem
        Private menuImage As System.Windows.Forms.MenuItem
        Private menuDraw As System.Windows.Forms.MenuItem
        Private menuCommands As System.Windows.Forms.MenuItem
        Private menuChannels As System.Windows.Forms.MenuItem
        Private menuEffects As System.Windows.Forms.MenuItem
        Private menuFilters As System.Windows.Forms.MenuItem
        Private menuTransforms As System.Windows.Forms.MenuItem
        Private menuDocument As System.Windows.Forms.MenuItem
        Private menuItem16 As System.Windows.Forms.MenuItem
        Private menuFlip As System.Windows.Forms.MenuItem
        Private menuItem19 As System.Windows.Forms.MenuItem
        Private WithEvents menuPush As System.Windows.Forms.MenuItem
        Private WithEvents menuQuadrilateralWarp As System.Windows.Forms.MenuItem
        Private WithEvents menuSkew As System.Windows.Forms.MenuItem
        Private menuItem23 As System.Windows.Forms.MenuItem
        Private WithEvents menuRotate As System.Windows.Forms.MenuItem
        Private WithEvents menuResample As System.Windows.Forms.MenuItem
        Private WithEvents menuCrop As System.Windows.Forms.MenuItem
        Private WithEvents menuAutoCrop As System.Windows.Forms.MenuItem
        Private menuItem28 As System.Windows.Forms.MenuItem
        Private WithEvents menuOverlay As System.Windows.Forms.MenuItem
        Private WithEvents menuResizeCanvas As System.Windows.Forms.MenuItem
        Private WithEvents menuFlipHorizontal As System.Windows.Forms.MenuItem
        Private WithEvents menuFlipVertical As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsCombine As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsReplace As System.Windows.Forms.MenuItem
        Private menuItem13 As System.Windows.Forms.MenuItem
        Private menuItem24 As System.Windows.Forms.MenuItem
        Private menuItem46 As System.Windows.Forms.MenuItem
        Private menuItem58 As System.Windows.Forms.MenuItem
        Private menuItem70 As System.Windows.Forms.MenuItem
        Private menuItem73 As System.Windows.Forms.MenuItem
        Private menuItem77 As System.Windows.Forms.MenuItem
        Private menuItem94 As System.Windows.Forms.MenuItem
        Private menuItem103 As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsSplit As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsAdjust As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsAdjustHsl As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsApplyLut As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsInvert As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsShift As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsSwap As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsFlattenAlpha As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsAphaColor As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsAlphaMask As System.Windows.Forms.MenuItem
        Private WithEvents menuChannelsAlphaValue As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsAdjustTint As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsBevelEdge As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsCrackle As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsDropShadow As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsFingerPrint As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsFloodFill As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsGamma As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsGauzy As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsHalftone As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsMosaic As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsPosterize As System.Windows.Forms.MenuItem
        Private WithEvents menuReduceColors As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsReplaceColor As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsSolarize As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsStipple As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsTintGrayscale As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsBrightnessHistogramEqualize As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsBrightnessHistogramStretch As System.Windows.Forms.MenuItem
        Private WithEvents menuHistogramEqualize As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsHistogramStretch As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersBrightnessContrast As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersSaturation As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersBlur As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersGaussianBlur As System.Windows.Forms.MenuItem
        Private WithEvents menuAdaptiveUnsharpMask As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsUnsharpMask As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersSharpen As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersAddNoise As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersEmboss As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersIntensify As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersHighPass As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersMaximum As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersMean As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersMedian As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersMidpoint As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersMinimum As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersMorphological As System.Windows.Forms.MenuItem
        Private WithEvents menuMorphoErosion As System.Windows.Forms.MenuItem
        Private WithEvents menuMorphoDilation As System.Windows.Forms.MenuItem
        Private WithEvents menuMorphoOpen As System.Windows.Forms.MenuItem
        Private WithEvents menuMorphoClose As System.Windows.Forms.MenuItem
        Private WithEvents menuMorphoGradient As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersThreshold As System.Windows.Forms.MenuItem
        Private WithEvents menuMorphoTophat As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersConvolutionFilter As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersConvolutionMatrix As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersCannyEdgeDetector As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsChain As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsApply As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsBumpMap As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsElliptical As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsLens As System.Windows.Forms.MenuItem
        Private WithEvents menuLineSlice As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsMarble As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsOffset As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsPerlin As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsPinch As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsPolygon As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsRandom As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsRipple As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsSpin As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformSpinWave As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsWave As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsWow As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsZigZag As System.Windows.Forms.MenuItem
        Private WithEvents menuTransformsUser As System.Windows.Forms.MenuItem
        Private WithEvents menuDocumentAutoDeskew As System.Windows.Forms.MenuItem
        Private WithEvents menuDocumentMedian As System.Windows.Forms.MenuItem
        Private WithEvents menuDocumentMorphological As System.Windows.Forms.MenuItem
        Private WithEvents menuBinaryErosion As System.Windows.Forms.MenuItem
        Private WithEvents menuBinaryDilation As System.Windows.Forms.MenuItem
        Private WithEvents menuBinaryOpen As System.Windows.Forms.MenuItem
        Private WithEvents menuBinaryClose As System.Windows.Forms.MenuItem
        Private WithEvents menuBinaryBoundary As System.Windows.Forms.MenuItem
        Private WithEvents menuDocumentThinning As System.Windows.Forms.MenuItem
        Private WithEvents menuDocumentHitOrMiss As System.Windows.Forms.MenuItem
        Private WithEvents menuOverlayNormal As System.Windows.Forms.MenuItem
        Private WithEvents menuOverlayMasked As System.Windows.Forms.MenuItem
        Private WithEvents menuOverlayMerged As System.Windows.Forms.MenuItem
        Private WithEvents menuDrawLine As System.Windows.Forms.MenuItem
        Private WithEvents menuDrawLines As System.Windows.Forms.MenuItem
        Private WithEvents menuDrawRectangle As System.Windows.Forms.MenuItem
        Private menuItem21 As System.Windows.Forms.MenuItem
        Private WithEvents menuDrawEllipse As System.Windows.Forms.MenuItem
        Private WithEvents menuDrawFreehand As System.Windows.Forms.MenuItem
        Private WithEvents menuDrawPolygon As System.Windows.Forms.MenuItem
        Private WithEvents menuDrawText As System.Windows.Forms.MenuItem
        Private WithEvents menuDrawSetBackcolor As System.Windows.Forms.MenuItem
        Private WithEvents menuDrawClearBackcolor As System.Windows.Forms.MenuItem
        Private WithEvents menuFileDecoders As System.Windows.Forms.MenuItem
        Private WithEvents menuFileJPEG As System.Windows.Forms.MenuItem
        Private WithEvents menuFileTLA As System.Windows.Forms.MenuItem
        Private WithEvents menuFilePNG As System.Windows.Forms.MenuItem
        Private WithEvents menuFileWMF As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersDespeckle As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersEdge As System.Windows.Forms.MenuItem
        Private menuItem5 As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsDeInterlace As System.Windows.Forms.MenuItem
        Private WithEvents menuFiltersDustScratchRemoval As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsOilPaint As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsWatercolorTint As System.Windows.Forms.MenuItem
        Private WithEvents menuDocumentDespeckle As System.Windows.Forms.MenuItem
        Private menuItem3 As System.Windows.Forms.MenuItem
        Private WithEvents menuFftBandPass As System.Windows.Forms.MenuItem
        Private WithEvents menuFftButterworthHighBoost As System.Windows.Forms.MenuItem
        Private WithEvents menuFftButterworthHighPass As System.Windows.Forms.MenuItem
        Private WithEvents menuFftButterworthLowPass As System.Windows.Forms.MenuItem
        Private WithEvents menuFftGaussianHighBoost As System.Windows.Forms.MenuItem
        Private WithEvents menuFftGaussianHighPass As System.Windows.Forms.MenuItem
        Private WithEvents menuFftGaussianLowPass As System.Windows.Forms.MenuItem
        Private WithEvents menuFftIdealHighPass As System.Windows.Forms.MenuItem
        Private WithEvents menuFftIdealLowPass As System.Windows.Forms.MenuItem
        Private WithEvents menuFftInversePower As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsRedEyeRemoval As System.Windows.Forms.MenuItem
        Private menuItem4 As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsLevels As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsAutoLevels As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsAutoContrast As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsAutoColor As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsAutoWhiteBalance As System.Windows.Forms.MenuItem
        Private WithEvents menuImageZoom132 As System.Windows.Forms.MenuItem
        Private WithEvents menuImageZoom116 As System.Windows.Forms.MenuItem
        Private WithEvents menuImageZoom18 As System.Windows.Forms.MenuItem
        Private WithEvents menuImageZoom31 As System.Windows.Forms.MenuItem
        Private menuThresholding As System.Windows.Forms.MenuItem
        Private WithEvents menuThresholdAdaptive As System.Windows.Forms.MenuItem
        Private WithEvents menuThresholdGlobal As System.Windows.Forms.MenuItem
        Private WithEvents menuBorderRemoval As System.Windows.Forms.MenuItem
        Private menuHelp As System.Windows.Forms.MenuItem
        Private WithEvents menuItem15 As System.Windows.Forms.MenuItem
        Private polygonPoints As Point()
        Private statusBarLoadTime As System.Windows.Forms.StatusBarPanel
        Private WithEvents menuFilePDF As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsRoundedBevel As System.Windows.Forms.MenuItem
        Private WithEvents menuEffectsSaturation As System.Windows.Forms.MenuItem
        Private WithEvents cboFrameIndex As System.Windows.Forms.ComboBox
        Friend WithEvents menuThresholdDynamic As System.Windows.Forms.MenuItem
        Friend WithEvents menuDithering As System.Windows.Forms.MenuItem
        Private _startTick As Integer

#End Region

        Public Sub New()
            '
            ' Required for Windows Form Designer support
            '
            InitializeComponent()

            'hook up event code to show a message when the pixel format changes
            AddHandler AtalaImage.ChangePixelFormat, AddressOf AtalaImage_ChangePixelFormat

            HelperMethods.PopulateDecoders(RegisteredDecoders.Decoders)

            If AtalaImage.Edition <> LicenseEdition.Document Then
                Me.menuDocument.Enabled = False
            End If

            ' Scroll with the mouse wheel.
            AddHandler Viewer.MouseWheel, AddressOf Viewer_MouseWheel
        End Sub


#Region "Windows Form Designer generated code"
        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            For Each filename As String In _tempFiles
                File.Delete(filename)
            Next

            _tempFiles.Clear()

            If disposing Then
                If (Not is_Disposed) Then
                    is_Disposed = True
                End If

                If Not components Is Nothing Then
                    components.Dispose()
                End If
            End If

            MyBase.Dispose(disposing)
        End Sub


        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormMain))
            Me.statusInfo = New System.Windows.Forms.StatusBar
            Me.statusBarPosition = New System.Windows.Forms.StatusBarPanel
            Me.statusBarLoadTime = New System.Windows.Forms.StatusBarPanel
            Me.statusBarMessage = New System.Windows.Forms.StatusBarPanel
            Me.statusBarProgress = New System.Windows.Forms.StatusBarPanel
            Me.toolBar1 = New System.Windows.Forms.ToolBar
            Me.tbOpen = New System.Windows.Forms.ToolBarButton
            Me.tbSave = New System.Windows.Forms.ToolBarButton
            Me.tbSep = New System.Windows.Forms.ToolBarButton
            Me.tbUndo = New System.Windows.Forms.ToolBarButton
            Me.tbRedo = New System.Windows.Forms.ToolBarButton
            Me.toolBarButton1 = New System.Windows.Forms.ToolBarButton
            Me.tbArrow = New System.Windows.Forms.ToolBarButton
            Me.tbSelectRectangle = New System.Windows.Forms.ToolBarButton
            Me.tbSelectEllipse = New System.Windows.Forms.ToolBarButton
            Me.tbPan = New System.Windows.Forms.ToolBarButton
            Me.tbMagnifier = New System.Windows.Forms.ToolBarButton
            Me.tbZoom = New System.Windows.Forms.ToolBarButton
            Me.tbZoomSelection = New System.Windows.Forms.ToolBarButton
            Me.imageList1 = New System.Windows.Forms.ImageList(Me.components)
            Me.progressBar1 = New System.Windows.Forms.ProgressBar
            Me.pageSetupDialog1 = New System.Windows.Forms.PageSetupDialog
            Me.printDialog1 = New System.Windows.Forms.PrintDialog
            Me.rectangleSelection = New Atalasoft.Imaging.WinControls.RectangleSelection
            Me.Viewer = New Atalasoft.Imaging.WinControls.WorkspaceViewer
            Me.imagePrintDocument1 = New Atalasoft.Imaging.WinControls.ImagePrintDocument
            Me.ellipseRubberband = New Atalasoft.Imaging.WinControls.EllipseRubberband
            Me.rectangleDraw = New Atalasoft.Imaging.WinControls.RectangleRubberband
            Me.ellipseDraw = New Atalasoft.Imaging.WinControls.EllipseRubberband
            Me.lineDraw = New Atalasoft.Imaging.WinControls.LineRubberband
            Me.mainMenu = New System.Windows.Forms.MainMenu(Me.components)
            Me.menuFile = New System.Windows.Forms.MenuItem
            Me.menuFileNew = New System.Windows.Forms.MenuItem
            Me.menuFileOpen = New System.Windows.Forms.MenuItem
            Me.menuFileOpenFromURL = New System.Windows.Forms.MenuItem
            Me.menuFileDecoders = New System.Windows.Forms.MenuItem
            Me.menuFileJPEG = New System.Windows.Forms.MenuItem
            Me.menuFilePDF = New System.Windows.Forms.MenuItem
            Me.menuFilePNG = New System.Windows.Forms.MenuItem
            Me.menuFileTLA = New System.Windows.Forms.MenuItem
            Me.menuFileWMF = New System.Windows.Forms.MenuItem
            Me.menuItem1 = New System.Windows.Forms.MenuItem
            Me.menuFileNoise = New System.Windows.Forms.MenuItem
            Me.menuItem2 = New System.Windows.Forms.MenuItem
            Me.menuFileSaveAs = New System.Windows.Forms.MenuItem
            Me.menuFileSaveFTP = New System.Windows.Forms.MenuItem
            Me.menuItem5 = New System.Windows.Forms.MenuItem
            Me.menuFilePageSetup = New System.Windows.Forms.MenuItem
            Me.menuFilePrintImage = New System.Windows.Forms.MenuItem
            Me.menuItem6 = New System.Windows.Forms.MenuItem
            Me.menuFileExit = New System.Windows.Forms.MenuItem
            Me.menuEdit = New System.Windows.Forms.MenuItem
            Me.menuEditUndo = New System.Windows.Forms.MenuItem
            Me.menuEditRedo = New System.Windows.Forms.MenuItem
            Me.menuItem8 = New System.Windows.Forms.MenuItem
            Me.menuEditCut = New System.Windows.Forms.MenuItem
            Me.menuEditCopy = New System.Windows.Forms.MenuItem
            Me.menuEditPaste = New System.Windows.Forms.MenuItem
            Me.menuItem12 = New System.Windows.Forms.MenuItem
            Me.menuEditOptions = New System.Windows.Forms.MenuItem
            Me.menuImage = New System.Windows.Forms.MenuItem
            Me.menuImageInformation = New System.Windows.Forms.MenuItem
            Me.menuImageChangePixelFormat = New System.Windows.Forms.MenuItem
            Me.menuImageShowHistogram = New System.Windows.Forms.MenuItem
            Me.menuItem11 = New System.Windows.Forms.MenuItem
            Me.menuImageExif = New System.Windows.Forms.MenuItem
            Me.menuImageIptc = New System.Windows.Forms.MenuItem
            Me.menuItem7 = New System.Windows.Forms.MenuItem
            Me.menuImageZoom = New System.Windows.Forms.MenuItem
            Me.menuImageZoom132 = New System.Windows.Forms.MenuItem
            Me.menuImageZoom116 = New System.Windows.Forms.MenuItem
            Me.menuImageZoom18 = New System.Windows.Forms.MenuItem
            Me.menuImageZoom25 = New System.Windows.Forms.MenuItem
            Me.menuImageZoom50 = New System.Windows.Forms.MenuItem
            Me.menuImageZoom100 = New System.Windows.Forms.MenuItem
            Me.menuImageZoom200 = New System.Windows.Forms.MenuItem
            Me.menuImageZoom500 = New System.Windows.Forms.MenuItem
            Me.menuImageZoom1000 = New System.Windows.Forms.MenuItem
            Me.menuImageZoom31 = New System.Windows.Forms.MenuItem
            Me.menuItem9 = New System.Windows.Forms.MenuItem
            Me.menuDraw = New System.Windows.Forms.MenuItem
            Me.menuDrawLine = New System.Windows.Forms.MenuItem
            Me.menuDrawLines = New System.Windows.Forms.MenuItem
            Me.menuDrawRectangle = New System.Windows.Forms.MenuItem
            Me.menuDrawEllipse = New System.Windows.Forms.MenuItem
            Me.menuDrawPolygon = New System.Windows.Forms.MenuItem
            Me.menuDrawFreehand = New System.Windows.Forms.MenuItem
            Me.menuItem21 = New System.Windows.Forms.MenuItem
            Me.menuDrawText = New System.Windows.Forms.MenuItem
            Me.menuDrawSetBackcolor = New System.Windows.Forms.MenuItem
            Me.menuDrawClearBackcolor = New System.Windows.Forms.MenuItem
            Me.menuCommands = New System.Windows.Forms.MenuItem
            Me.menuChannels = New System.Windows.Forms.MenuItem
            Me.menuChannelsCombine = New System.Windows.Forms.MenuItem
            Me.menuChannelsReplace = New System.Windows.Forms.MenuItem
            Me.menuChannelsSplit = New System.Windows.Forms.MenuItem
            Me.menuItem13 = New System.Windows.Forms.MenuItem
            Me.menuChannelsAdjust = New System.Windows.Forms.MenuItem
            Me.menuChannelsAdjustHsl = New System.Windows.Forms.MenuItem
            Me.menuChannelsApplyLut = New System.Windows.Forms.MenuItem
            Me.menuChannelsInvert = New System.Windows.Forms.MenuItem
            Me.menuChannelsShift = New System.Windows.Forms.MenuItem
            Me.menuChannelsSwap = New System.Windows.Forms.MenuItem
            Me.menuItem24 = New System.Windows.Forms.MenuItem
            Me.menuChannelsFlattenAlpha = New System.Windows.Forms.MenuItem
            Me.menuChannelsAphaColor = New System.Windows.Forms.MenuItem
            Me.menuChannelsAlphaMask = New System.Windows.Forms.MenuItem
            Me.menuChannelsAlphaValue = New System.Windows.Forms.MenuItem
            Me.menuEffects = New System.Windows.Forms.MenuItem
            Me.menuEffectsAdjustTint = New System.Windows.Forms.MenuItem
            Me.menuEffectsBevelEdge = New System.Windows.Forms.MenuItem
            Me.menuEffectsCrackle = New System.Windows.Forms.MenuItem
            Me.menuEffectsDeInterlace = New System.Windows.Forms.MenuItem
            Me.menuEffectsDropShadow = New System.Windows.Forms.MenuItem
            Me.menuEffectsFingerPrint = New System.Windows.Forms.MenuItem
            Me.menuEffectsFloodFill = New System.Windows.Forms.MenuItem
            Me.menuEffectsGamma = New System.Windows.Forms.MenuItem
            Me.menuEffectsGauzy = New System.Windows.Forms.MenuItem
            Me.menuEffectsHalftone = New System.Windows.Forms.MenuItem
            Me.menuEffectsMosaic = New System.Windows.Forms.MenuItem
            Me.menuEffectsOilPaint = New System.Windows.Forms.MenuItem
            Me.menuEffectsPosterize = New System.Windows.Forms.MenuItem
            Me.menuReduceColors = New System.Windows.Forms.MenuItem
            Me.menuEffectsReplaceColor = New System.Windows.Forms.MenuItem
            Me.menuEffectsRoundedBevel = New System.Windows.Forms.MenuItem
            Me.menuEffectsSaturation = New System.Windows.Forms.MenuItem
            Me.menuEffectsSolarize = New System.Windows.Forms.MenuItem
            Me.menuEffectsStipple = New System.Windows.Forms.MenuItem
            Me.menuEffectsTintGrayscale = New System.Windows.Forms.MenuItem
            Me.menuEffectsWatercolorTint = New System.Windows.Forms.MenuItem
            Me.menuItem46 = New System.Windows.Forms.MenuItem
            Me.menuEffectsBrightnessHistogramEqualize = New System.Windows.Forms.MenuItem
            Me.menuEffectsBrightnessHistogramStretch = New System.Windows.Forms.MenuItem
            Me.menuHistogramEqualize = New System.Windows.Forms.MenuItem
            Me.menuEffectsHistogramStretch = New System.Windows.Forms.MenuItem
            Me.menuItem4 = New System.Windows.Forms.MenuItem
            Me.menuEffectsRedEyeRemoval = New System.Windows.Forms.MenuItem
            Me.menuEffectsLevels = New System.Windows.Forms.MenuItem
            Me.menuEffectsAutoLevels = New System.Windows.Forms.MenuItem
            Me.menuEffectsAutoContrast = New System.Windows.Forms.MenuItem
            Me.menuEffectsAutoColor = New System.Windows.Forms.MenuItem
            Me.menuEffectsAutoWhiteBalance = New System.Windows.Forms.MenuItem
            Me.menuFilters = New System.Windows.Forms.MenuItem
            Me.menuFiltersBrightnessContrast = New System.Windows.Forms.MenuItem
            Me.menuFiltersSaturation = New System.Windows.Forms.MenuItem
            Me.menuFiltersBlur = New System.Windows.Forms.MenuItem
            Me.menuFiltersGaussianBlur = New System.Windows.Forms.MenuItem
            Me.menuAdaptiveUnsharpMask = New System.Windows.Forms.MenuItem
            Me.menuEffectsUnsharpMask = New System.Windows.Forms.MenuItem
            Me.menuFiltersSharpen = New System.Windows.Forms.MenuItem
            Me.menuItem58 = New System.Windows.Forms.MenuItem
            Me.menuFiltersAddNoise = New System.Windows.Forms.MenuItem
            Me.menuFiltersDespeckle = New System.Windows.Forms.MenuItem
            Me.menuFiltersEmboss = New System.Windows.Forms.MenuItem
            Me.menuFiltersIntensify = New System.Windows.Forms.MenuItem
            Me.menuFiltersHighPass = New System.Windows.Forms.MenuItem
            Me.menuFiltersMaximum = New System.Windows.Forms.MenuItem
            Me.menuFiltersMean = New System.Windows.Forms.MenuItem
            Me.menuFiltersMedian = New System.Windows.Forms.MenuItem
            Me.menuFiltersMidpoint = New System.Windows.Forms.MenuItem
            Me.menuFiltersMinimum = New System.Windows.Forms.MenuItem
            Me.menuFiltersMorphological = New System.Windows.Forms.MenuItem
            Me.menuMorphoDilation = New System.Windows.Forms.MenuItem
            Me.menuMorphoErosion = New System.Windows.Forms.MenuItem
            Me.menuMorphoOpen = New System.Windows.Forms.MenuItem
            Me.menuMorphoClose = New System.Windows.Forms.MenuItem
            Me.menuMorphoTophat = New System.Windows.Forms.MenuItem
            Me.menuMorphoGradient = New System.Windows.Forms.MenuItem
            Me.menuFiltersThreshold = New System.Windows.Forms.MenuItem
            Me.menuItem70 = New System.Windows.Forms.MenuItem
            Me.menuFiltersConvolutionFilter = New System.Windows.Forms.MenuItem
            Me.menuFiltersConvolutionMatrix = New System.Windows.Forms.MenuItem
            Me.menuItem73 = New System.Windows.Forms.MenuItem
            Me.menuFiltersEdge = New System.Windows.Forms.MenuItem
            Me.menuFiltersCannyEdgeDetector = New System.Windows.Forms.MenuItem
            Me.menuFiltersDustScratchRemoval = New System.Windows.Forms.MenuItem
            Me.menuTransforms = New System.Windows.Forms.MenuItem
            Me.menuTransformsChain = New System.Windows.Forms.MenuItem
            Me.menuTransformsApply = New System.Windows.Forms.MenuItem
            Me.menuItem77 = New System.Windows.Forms.MenuItem
            Me.menuTransformsBumpMap = New System.Windows.Forms.MenuItem
            Me.menuTransformsElliptical = New System.Windows.Forms.MenuItem
            Me.menuTransformsLens = New System.Windows.Forms.MenuItem
            Me.menuLineSlice = New System.Windows.Forms.MenuItem
            Me.menuTransformsMarble = New System.Windows.Forms.MenuItem
            Me.menuTransformsOffset = New System.Windows.Forms.MenuItem
            Me.menuTransformsPerlin = New System.Windows.Forms.MenuItem
            Me.menuTransformsPinch = New System.Windows.Forms.MenuItem
            Me.menuTransformsPolygon = New System.Windows.Forms.MenuItem
            Me.menuTransformsRandom = New System.Windows.Forms.MenuItem
            Me.menuTransformsRipple = New System.Windows.Forms.MenuItem
            Me.menuTransformsSpin = New System.Windows.Forms.MenuItem
            Me.menuTransformSpinWave = New System.Windows.Forms.MenuItem
            Me.menuTransformsWave = New System.Windows.Forms.MenuItem
            Me.menuTransformsWow = New System.Windows.Forms.MenuItem
            Me.menuTransformsZigZag = New System.Windows.Forms.MenuItem
            Me.menuItem94 = New System.Windows.Forms.MenuItem
            Me.menuTransformsUser = New System.Windows.Forms.MenuItem
            Me.menuDocument = New System.Windows.Forms.MenuItem
            Me.menuDocumentAutoDeskew = New System.Windows.Forms.MenuItem
            Me.menuDocumentMedian = New System.Windows.Forms.MenuItem
            Me.menuDocumentDespeckle = New System.Windows.Forms.MenuItem
            Me.menuItem103 = New System.Windows.Forms.MenuItem
            Me.menuDocumentMorphological = New System.Windows.Forms.MenuItem
            Me.menuBinaryDilation = New System.Windows.Forms.MenuItem
            Me.menuBinaryErosion = New System.Windows.Forms.MenuItem
            Me.menuBinaryOpen = New System.Windows.Forms.MenuItem
            Me.menuBinaryClose = New System.Windows.Forms.MenuItem
            Me.menuBinaryBoundary = New System.Windows.Forms.MenuItem
            Me.menuDocumentThinning = New System.Windows.Forms.MenuItem
            Me.menuDocumentHitOrMiss = New System.Windows.Forms.MenuItem
            Me.menuThresholding = New System.Windows.Forms.MenuItem
            Me.menuThresholdAdaptive = New System.Windows.Forms.MenuItem
            Me.menuThresholdGlobal = New System.Windows.Forms.MenuItem
            Me.menuThresholdDynamic = New System.Windows.Forms.MenuItem
            Me.menuBorderRemoval = New System.Windows.Forms.MenuItem
            Me.menuItem3 = New System.Windows.Forms.MenuItem
            Me.menuFftBandPass = New System.Windows.Forms.MenuItem
            Me.menuFftButterworthHighBoost = New System.Windows.Forms.MenuItem
            Me.menuFftButterworthHighPass = New System.Windows.Forms.MenuItem
            Me.menuFftButterworthLowPass = New System.Windows.Forms.MenuItem
            Me.menuFftGaussianHighBoost = New System.Windows.Forms.MenuItem
            Me.menuFftGaussianHighPass = New System.Windows.Forms.MenuItem
            Me.menuFftGaussianLowPass = New System.Windows.Forms.MenuItem
            Me.menuFftIdealHighPass = New System.Windows.Forms.MenuItem
            Me.menuFftIdealLowPass = New System.Windows.Forms.MenuItem
            Me.menuFftInversePower = New System.Windows.Forms.MenuItem
            Me.menuItem16 = New System.Windows.Forms.MenuItem
            Me.menuFlip = New System.Windows.Forms.MenuItem
            Me.menuFlipHorizontal = New System.Windows.Forms.MenuItem
            Me.menuFlipVertical = New System.Windows.Forms.MenuItem
            Me.menuItem23 = New System.Windows.Forms.MenuItem
            Me.menuRotate = New System.Windows.Forms.MenuItem
            Me.menuResizeCanvas = New System.Windows.Forms.MenuItem
            Me.menuResample = New System.Windows.Forms.MenuItem
            Me.menuCrop = New System.Windows.Forms.MenuItem
            Me.menuAutoCrop = New System.Windows.Forms.MenuItem
            Me.menuSkew = New System.Windows.Forms.MenuItem
            Me.menuQuadrilateralWarp = New System.Windows.Forms.MenuItem
            Me.menuPush = New System.Windows.Forms.MenuItem
            Me.menuItem28 = New System.Windows.Forms.MenuItem
            Me.menuOverlay = New System.Windows.Forms.MenuItem
            Me.menuOverlayNormal = New System.Windows.Forms.MenuItem
            Me.menuOverlayMasked = New System.Windows.Forms.MenuItem
            Me.menuOverlayMerged = New System.Windows.Forms.MenuItem
            Me.menuItem19 = New System.Windows.Forms.MenuItem
            Me.menuHelp = New System.Windows.Forms.MenuItem
            Me.menuItem15 = New System.Windows.Forms.MenuItem
            Me.cboFrameIndex = New System.Windows.Forms.ComboBox
            Me.menuDithering = New System.Windows.Forms.MenuItem
            CType(Me.statusBarPosition, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.statusBarLoadTime, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.statusBarMessage, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.statusBarProgress, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'statusInfo
            '
            Me.statusInfo.Location = New System.Drawing.Point(0, 336)
            Me.statusInfo.Name = "statusInfo"
            Me.statusInfo.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.statusBarPosition, Me.statusBarLoadTime, Me.statusBarMessage, Me.statusBarProgress})
            Me.statusInfo.ShowPanels = True
            Me.statusInfo.Size = New System.Drawing.Size(552, 22)
            Me.statusInfo.TabIndex = 1
            Me.statusInfo.Text = "statusInfo"
            '
            'statusBarPosition
            '
            Me.statusBarPosition.Alignment = System.Windows.Forms.HorizontalAlignment.Center
            Me.statusBarPosition.MinWidth = 125
            Me.statusBarPosition.Name = "statusBarPosition"
            Me.statusBarPosition.Text = "-- x --"
            Me.statusBarPosition.Width = 125
            '
            'statusBarLoadTime
            '
            Me.statusBarLoadTime.Name = "statusBarLoadTime"
            Me.statusBarLoadTime.Width = 150
            '
            'statusBarMessage
            '
            Me.statusBarMessage.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring
            Me.statusBarMessage.Name = "statusBarMessage"
            Me.statusBarMessage.Text = "Atalasoft dotImage"
            Me.statusBarMessage.Width = 161
            '
            'statusBarProgress
            '
            Me.statusBarProgress.MinWidth = 100
            Me.statusBarProgress.Name = "statusBarProgress"
            '
            'toolBar1
            '
            Me.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat
            Me.toolBar1.AutoSize = False
            Me.toolBar1.Buttons.AddRange(New System.Windows.Forms.ToolBarButton() {Me.tbOpen, Me.tbSave, Me.tbSep, Me.tbUndo, Me.tbRedo, Me.toolBarButton1, Me.tbArrow, Me.tbSelectRectangle, Me.tbSelectEllipse, Me.tbPan, Me.tbMagnifier, Me.tbZoom, Me.tbZoomSelection})
            Me.toolBar1.DropDownArrows = True
            Me.toolBar1.ImageList = Me.imageList1
            Me.toolBar1.Location = New System.Drawing.Point(0, 0)
            Me.toolBar1.Name = "toolBar1"
            Me.toolBar1.ShowToolTips = True
            Me.toolBar1.Size = New System.Drawing.Size(552, 24)
            Me.toolBar1.TabIndex = 2
            Me.toolBar1.Wrappable = False
            '
            'tbOpen
            '
            Me.tbOpen.ImageIndex = 3
            Me.tbOpen.Name = "tbOpen"
            Me.tbOpen.ToolTipText = "Open"
            '
            'tbSave
            '
            Me.tbSave.Enabled = False
            Me.tbSave.ImageIndex = 4
            Me.tbSave.Name = "tbSave"
            Me.tbSave.ToolTipText = "Save"
            '
            'tbSep
            '
            Me.tbSep.Name = "tbSep"
            Me.tbSep.Style = System.Windows.Forms.ToolBarButtonStyle.Separator
            '
            'tbUndo
            '
            Me.tbUndo.Enabled = False
            Me.tbUndo.ImageIndex = 5
            Me.tbUndo.Name = "tbUndo"
            Me.tbUndo.ToolTipText = "Undo"
            '
            'tbRedo
            '
            Me.tbRedo.Enabled = False
            Me.tbRedo.ImageIndex = 9
            Me.tbRedo.Name = "tbRedo"
            Me.tbRedo.ToolTipText = "Redo"
            '
            'toolBarButton1
            '
            Me.toolBarButton1.Name = "toolBarButton1"
            Me.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator
            '
            'tbArrow
            '
            Me.tbArrow.ImageIndex = 0
            Me.tbArrow.Name = "tbArrow"
            Me.tbArrow.Pushed = True
            Me.tbArrow.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton
            Me.tbArrow.ToolTipText = "Arrow"
            '
            'tbSelectRectangle
            '
            Me.tbSelectRectangle.ImageIndex = 1
            Me.tbSelectRectangle.Name = "tbSelectRectangle"
            Me.tbSelectRectangle.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton
            Me.tbSelectRectangle.ToolTipText = "Rectangle Selection"
            '
            'tbSelectEllipse
            '
            Me.tbSelectEllipse.ImageIndex = 10
            Me.tbSelectEllipse.Name = "tbSelectEllipse"
            Me.tbSelectEllipse.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton
            Me.tbSelectEllipse.ToolTipText = "Ellipse Selection"
            '
            'tbPan
            '
            Me.tbPan.ImageIndex = 2
            Me.tbPan.Name = "tbPan"
            Me.tbPan.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton
            Me.tbPan.ToolTipText = "Pan"
            '
            'tbMagnifier
            '
            Me.tbMagnifier.ImageIndex = 6
            Me.tbMagnifier.Name = "tbMagnifier"
            Me.tbMagnifier.ToolTipText = "Magnifier"
            '
            'tbZoom
            '
            Me.tbZoom.ImageIndex = 7
            Me.tbZoom.Name = "tbZoom"
            Me.tbZoom.ToolTipText = "Zoom"
            '
            'tbZoomSelection
            '
            Me.tbZoomSelection.ImageIndex = 8
            Me.tbZoomSelection.Name = "tbZoomSelection"
            Me.tbZoomSelection.ToolTipText = "Zoom Selection"
            '
            'imageList1
            '
            Me.imageList1.ImageStream = CType(resources.GetObject("imageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
            Me.imageList1.TransparentColor = System.Drawing.Color.Transparent
            Me.imageList1.Images.SetKeyName(0, "")
            Me.imageList1.Images.SetKeyName(1, "")
            Me.imageList1.Images.SetKeyName(2, "")
            Me.imageList1.Images.SetKeyName(3, "")
            Me.imageList1.Images.SetKeyName(4, "")
            Me.imageList1.Images.SetKeyName(5, "")
            Me.imageList1.Images.SetKeyName(6, "")
            Me.imageList1.Images.SetKeyName(7, "")
            Me.imageList1.Images.SetKeyName(8, "")
            Me.imageList1.Images.SetKeyName(9, "")
            Me.imageList1.Images.SetKeyName(10, "")
            '
            'progressBar1
            '
            Me.progressBar1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.progressBar1.Location = New System.Drawing.Point(440, 340)
            Me.progressBar1.Name = "progressBar1"
            Me.progressBar1.Size = New System.Drawing.Size(100, 16)
            Me.progressBar1.TabIndex = 4
            '
            'rectangleSelection
            '
            Me.rectangleSelection.ActiveButtons = System.Windows.Forms.MouseButtons.Left
            Me.rectangleSelection.Animated = True
            Me.rectangleSelection.AspectRatio = 0.0!
            Me.rectangleSelection.BackgroundColor = System.Drawing.Color.White
            Me.rectangleSelection.ClickLock = False
            Me.rectangleSelection.Inverted = False
            Me.rectangleSelection.MoveCursor = System.Windows.Forms.Cursors.SizeAll
            Me.rectangleSelection.Parent = Me.Viewer
            Me.rectangleSelection.Pen.Color = System.Drawing.Color.Black
            Me.rectangleSelection.Pen.CustomDashPattern = New Integer() {8, 8}
            Me.rectangleSelection.Pen.LineStyle = Atalasoft.Imaging.Drawing.LineStyle.Custom
            Me.rectangleSelection.Persist = True
            Me.rectangleSelection.SelectionNESWCursor = System.Windows.Forms.Cursors.SizeNESW
            Me.rectangleSelection.SelectionNSCursor = System.Windows.Forms.Cursors.SizeNS
            Me.rectangleSelection.SelectionNWSECursor = System.Windows.Forms.Cursors.SizeNWSE
            Me.rectangleSelection.SelectionWECursor = System.Windows.Forms.Cursors.SizeWE
            '
            'Viewer
            '
            Me.Viewer.AllowDrop = True
            Me.Viewer.AntialiasDisplay = Atalasoft.Imaging.WinControls.AntialiasDisplayMode.ScaleToGray
            Me.Viewer.Asynchronous = True
            Me.Viewer.BackgroundImage = CType(resources.GetObject("Viewer.BackgroundImage"), System.Drawing.Image)
            Me.Viewer.Centered = True
            Me.Viewer.DisplayProfile = Nothing
            Me.Viewer.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Viewer.Location = New System.Drawing.Point(0, 24)
            Me.Viewer.Magnifier.BackColor = System.Drawing.Color.White
            Me.Viewer.Magnifier.BackgroundImage = CType(resources.GetObject("resource.BackgroundImage"), System.Drawing.Image)
            Me.Viewer.Magnifier.BorderColor = System.Drawing.Color.White
            Me.Viewer.Magnifier.Size = New System.Drawing.Size(100, 100)
            Me.Viewer.Name = "Viewer"
            Me.Viewer.OutputProfile = Nothing
            Me.Viewer.Selection = Me.rectangleSelection
            Me.Viewer.Size = New System.Drawing.Size(552, 312)
            Me.Viewer.TabIndex = 5
            Me.Viewer.Text = "workspaceViewer1"
            Me.Viewer.UndoLevels = 5
            '
            'imagePrintDocument1
            '
            Me.imagePrintDocument1.ScaleMode = Atalasoft.Imaging.WinControls.PrintScaleMode.None
            '
            'ellipseRubberband
            '
            Me.ellipseRubberband.ActiveButtons = System.Windows.Forms.MouseButtons.Left
            Me.ellipseRubberband.AspectRatio = 0.0!
            Me.ellipseRubberband.BackgroundColor = System.Drawing.Color.Transparent
            Me.ellipseRubberband.ClickLock = False
            Me.ellipseRubberband.ConstrainPosition = False
            Me.ellipseRubberband.Fill = Nothing
            Me.ellipseRubberband.MoveCursor = System.Windows.Forms.Cursors.SizeAll
            Me.ellipseRubberband.Parent = Me.Viewer
            Me.ellipseRubberband.Pen.Color = System.Drawing.Color.Black
            Me.ellipseRubberband.Pen.CustomDashPattern = New Integer() {8, 8}
            Me.ellipseRubberband.Persist = True
            Me.ellipseRubberband.SnapToPixelGrid = False
            '
            'rectangleDraw
            '
            Me.rectangleDraw.ActiveButtons = System.Windows.Forms.MouseButtons.Left
            Me.rectangleDraw.AspectRatio = 0.0!
            Me.rectangleDraw.BackgroundColor = System.Drawing.Color.Transparent
            Me.rectangleDraw.CornerRadius = New System.Drawing.Size(0, 0)
            Me.rectangleDraw.Fill = Nothing
            Me.rectangleDraw.Inverted = False
            Me.rectangleDraw.MoveCursor = System.Windows.Forms.Cursors.SizeAll
            Me.rectangleDraw.Parent = Me.Viewer
            Me.rectangleDraw.Pen.Color = System.Drawing.Color.Black
            Me.rectangleDraw.Pen.CustomDashPattern = New Integer() {8, 8}
            '
            'ellipseDraw
            '
            Me.ellipseDraw.ActiveButtons = System.Windows.Forms.MouseButtons.Left
            Me.ellipseDraw.AspectRatio = 0.0!
            Me.ellipseDraw.BackgroundColor = System.Drawing.Color.Transparent
            Me.ellipseDraw.ConstrainPosition = False
            Me.ellipseDraw.Fill = Nothing
            Me.ellipseDraw.Inverted = False
            Me.ellipseDraw.MoveCursor = System.Windows.Forms.Cursors.SizeAll
            Me.ellipseDraw.Parent = Me.Viewer
            Me.ellipseDraw.Pen.Color = System.Drawing.Color.Black
            Me.ellipseDraw.Pen.CustomDashPattern = New Integer() {8, 8}
            Me.ellipseDraw.Persist = True
            Me.ellipseDraw.SnapToPixelGrid = False
            '
            'lineDraw
            '
            Me.lineDraw.ActiveButtons = System.Windows.Forms.MouseButtons.Left
            Me.lineDraw.AspectRatio = 0.0!
            Me.lineDraw.BackgroundColor = System.Drawing.Color.Transparent
            Me.lineDraw.Inverted = False
            Me.lineDraw.MoveCursor = System.Windows.Forms.Cursors.SizeAll
            Me.lineDraw.Parent = Me.Viewer
            Me.lineDraw.Pen.Color = System.Drawing.Color.Black
            Me.lineDraw.Pen.CustomDashPattern = New Integer() {8, 8}
            '
            'mainMenu
            '
            Me.mainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFile, Me.menuEdit, Me.menuImage, Me.menuDraw, Me.menuCommands, Me.menuHelp})
            '
            'menuFile
            '
            Me.menuFile.Index = 0
            Me.menuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFileNew, Me.menuFileOpen, Me.menuFileOpenFromURL, Me.menuFileDecoders, Me.menuItem1, Me.menuFileNoise, Me.menuItem2, Me.menuFileSaveAs, Me.menuFileSaveFTP, Me.menuItem5, Me.menuFilePageSetup, Me.menuFilePrintImage, Me.menuItem6, Me.menuFileExit})
            Me.menuFile.Text = "&File"
            '
            'menuFileNew
            '
            Me.menuFileNew.Index = 0
            Me.menuFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN
            Me.menuFileNew.Text = "&New"
            '
            'menuFileOpen
            '
            Me.menuFileOpen.Index = 1
            Me.menuFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO
            Me.menuFileOpen.Text = "&Open"
            '
            'menuFileOpenFromURL
            '
            Me.menuFileOpenFromURL.Index = 2
            Me.menuFileOpenFromURL.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftO
            Me.menuFileOpenFromURL.Text = "Open from &URL"
            '
            'menuFileDecoders
            '
            Me.menuFileDecoders.Index = 3
            Me.menuFileDecoders.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFileJPEG, Me.menuFilePDF, Me.menuFilePNG, Me.menuFileTLA, Me.menuFileWMF})
            Me.menuFileDecoders.Text = "Decoder Settings"
            '
            'menuFileJPEG
            '
            Me.menuFileJPEG.Index = 0
            Me.menuFileJPEG.Text = "JPEG"
            '
            'menuFilePDF
            '
            Me.menuFilePDF.Index = 1
            Me.menuFilePDF.Text = "PDF"
            '
            'menuFilePNG
            '
            Me.menuFilePNG.Index = 2
            Me.menuFilePNG.Text = "PNG"
            '
            'menuFileTLA
            '
            Me.menuFileTLA.Index = 3
            Me.menuFileTLA.Text = "TLA"
            '
            'menuFileWMF
            '
            Me.menuFileWMF.Index = 4
            Me.menuFileWMF.Text = "WMF"
            '
            'menuItem1
            '
            Me.menuItem1.Index = 4
            Me.menuItem1.Text = "-"
            '
            'menuFileNoise
            '
            Me.menuFileNoise.Index = 5
            Me.menuFileNoise.Text = "Generate Noise Image"
            '
            'menuItem2
            '
            Me.menuItem2.Index = 6
            Me.menuItem2.Text = "-"
            '
            'menuFileSaveAs
            '
            Me.menuFileSaveAs.Enabled = False
            Me.menuFileSaveAs.Index = 7
            Me.menuFileSaveAs.Text = "Save &As"
            '
            'menuFileSaveFTP
            '
            Me.menuFileSaveFTP.Enabled = False
            Me.menuFileSaveFTP.Index = 8
            Me.menuFileSaveFTP.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftS
            Me.menuFileSaveFTP.Text = "Save to &FTP"
            '
            'menuItem5
            '
            Me.menuItem5.Index = 9
            Me.menuItem5.Text = "-"
            '
            'menuFilePageSetup
            '
            Me.menuFilePageSetup.Enabled = False
            Me.menuFilePageSetup.Index = 10
            Me.menuFilePageSetup.Text = "Page Setup"
            '
            'menuFilePrintImage
            '
            Me.menuFilePrintImage.Enabled = False
            Me.menuFilePrintImage.Index = 11
            Me.menuFilePrintImage.Text = "Print Image"
            '
            'menuItem6
            '
            Me.menuItem6.Index = 12
            Me.menuItem6.Text = "-"
            '
            'menuFileExit
            '
            Me.menuFileExit.Index = 13
            Me.menuFileExit.Text = "Exit"
            '
            'menuEdit
            '
            Me.menuEdit.Index = 1
            Me.menuEdit.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuEditUndo, Me.menuEditRedo, Me.menuItem8, Me.menuEditCut, Me.menuEditCopy, Me.menuEditPaste, Me.menuItem12, Me.menuEditOptions})
            Me.menuEdit.Text = "&Edit"
            '
            'menuEditUndo
            '
            Me.menuEditUndo.Enabled = False
            Me.menuEditUndo.Index = 0
            Me.menuEditUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ
            Me.menuEditUndo.Text = "&Undo"
            '
            'menuEditRedo
            '
            Me.menuEditRedo.Enabled = False
            Me.menuEditRedo.Index = 1
            Me.menuEditRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY
            Me.menuEditRedo.Text = "&Redo"
            '
            'menuItem8
            '
            Me.menuItem8.Index = 2
            Me.menuItem8.Text = "-"
            '
            'menuEditCut
            '
            Me.menuEditCut.Enabled = False
            Me.menuEditCut.Index = 3
            Me.menuEditCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX
            Me.menuEditCut.Text = "Cut"
            '
            'menuEditCopy
            '
            Me.menuEditCopy.Enabled = False
            Me.menuEditCopy.Index = 4
            Me.menuEditCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC
            Me.menuEditCopy.Text = "Copy"
            '
            'menuEditPaste
            '
            Me.menuEditPaste.Enabled = False
            Me.menuEditPaste.Index = 5
            Me.menuEditPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV
            Me.menuEditPaste.Text = "Paste"
            '
            'menuItem12
            '
            Me.menuItem12.Index = 6
            Me.menuItem12.Text = "-"
            '
            'menuEditOptions
            '
            Me.menuEditOptions.Index = 7
            Me.menuEditOptions.Text = "Options"
            '
            'menuImage
            '
            Me.menuImage.Enabled = False
            Me.menuImage.Index = 2
            Me.menuImage.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuImageInformation, Me.menuImageChangePixelFormat, Me.menuImageShowHistogram, Me.menuItem11, Me.menuImageExif, Me.menuImageIptc, Me.menuItem7, Me.menuImageZoom, Me.menuItem9})
            Me.menuImage.Text = "&Image"
            '
            'menuImageInformation
            '
            Me.menuImageInformation.Index = 0
            Me.menuImageInformation.Text = "Information"
            '
            'menuImageChangePixelFormat
            '
            Me.menuImageChangePixelFormat.Index = 1
            Me.menuImageChangePixelFormat.Text = "Change Pixel Format"
            '
            'menuImageShowHistogram
            '
            Me.menuImageShowHistogram.Index = 2
            Me.menuImageShowHistogram.Text = "Show Histogram"
            '
            'menuItem11
            '
            Me.menuItem11.Index = 3
            Me.menuItem11.Text = "-"
            '
            'menuImageExif
            '
            Me.menuImageExif.Index = 4
            Me.menuImageExif.Text = "&EXIF Data"
            '
            'menuImageIptc
            '
            Me.menuImageIptc.Index = 5
            Me.menuImageIptc.Text = "I&PTC Data"
            '
            'menuItem7
            '
            Me.menuItem7.Index = 6
            Me.menuItem7.Text = "-"
            '
            'menuImageZoom
            '
            Me.menuImageZoom.Index = 7
            Me.menuImageZoom.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuImageZoom132, Me.menuImageZoom116, Me.menuImageZoom18, Me.menuImageZoom25, Me.menuImageZoom50, Me.menuImageZoom100, Me.menuImageZoom200, Me.menuImageZoom500, Me.menuImageZoom1000, Me.menuImageZoom31})
            Me.menuImageZoom.Text = "Zoom"
            '
            'menuImageZoom132
            '
            Me.menuImageZoom132.Index = 0
            Me.menuImageZoom132.Text = "1/32"
            '
            'menuImageZoom116
            '
            Me.menuImageZoom116.Index = 1
            Me.menuImageZoom116.Text = "1/16"
            '
            'menuImageZoom18
            '
            Me.menuImageZoom18.Index = 2
            Me.menuImageZoom18.Text = "1/8"
            '
            'menuImageZoom25
            '
            Me.menuImageZoom25.Index = 3
            Me.menuImageZoom25.Text = "25%"
            '
            'menuImageZoom50
            '
            Me.menuImageZoom50.Index = 4
            Me.menuImageZoom50.Text = "50%"
            '
            'menuImageZoom100
            '
            Me.menuImageZoom100.Index = 5
            Me.menuImageZoom100.Text = "100%"
            '
            'menuImageZoom200
            '
            Me.menuImageZoom200.Index = 6
            Me.menuImageZoom200.Text = "200%"
            '
            'menuImageZoom500
            '
            Me.menuImageZoom500.Index = 7
            Me.menuImageZoom500.Text = "500%"
            '
            'menuImageZoom1000
            '
            Me.menuImageZoom1000.Index = 8
            Me.menuImageZoom1000.Text = "1000%"
            '
            'menuImageZoom31
            '
            Me.menuImageZoom31.Index = 9
            Me.menuImageZoom31.Text = "1/31"
            '
            'menuItem9
            '
            Me.menuItem9.Index = 8
            Me.menuItem9.Text = "-"
            '
            'menuDraw
            '
            Me.menuDraw.Enabled = False
            Me.menuDraw.Index = 3
            Me.menuDraw.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuDrawLine, Me.menuDrawLines, Me.menuDrawRectangle, Me.menuDrawEllipse, Me.menuDrawPolygon, Me.menuDrawFreehand, Me.menuItem21, Me.menuDrawText, Me.menuDrawSetBackcolor, Me.menuDrawClearBackcolor})
            Me.menuDraw.Text = "&Draw"
            '
            'menuDrawLine
            '
            Me.menuDrawLine.Index = 0
            Me.menuDrawLine.MergeType = System.Windows.Forms.MenuMerge.Replace
            Me.menuDrawLine.Text = "Line"
            '
            'menuDrawLines
            '
            Me.menuDrawLines.Index = 1
            Me.menuDrawLines.Text = "Lines"
            '
            'menuDrawRectangle
            '
            Me.menuDrawRectangle.Index = 2
            Me.menuDrawRectangle.Text = "Rectangle"
            '
            'menuDrawEllipse
            '
            Me.menuDrawEllipse.Index = 3
            Me.menuDrawEllipse.Text = "Ellipse"
            '
            'menuDrawPolygon
            '
            Me.menuDrawPolygon.Index = 4
            Me.menuDrawPolygon.Text = "Polygon"
            '
            'menuDrawFreehand
            '
            Me.menuDrawFreehand.Index = 5
            Me.menuDrawFreehand.Text = "Freehand"
            '
            'menuItem21
            '
            Me.menuItem21.Index = 6
            Me.menuItem21.Text = "-"
            '
            'menuDrawText
            '
            Me.menuDrawText.Index = 7
            Me.menuDrawText.Text = "Text"
            '
            'menuDrawSetBackcolor
            '
            Me.menuDrawSetBackcolor.Index = 8
            Me.menuDrawSetBackcolor.Text = "Set Text Backcolor..."
            '
            'menuDrawClearBackcolor
            '
            Me.menuDrawClearBackcolor.Index = 9
            Me.menuDrawClearBackcolor.Text = "Clear Text Backcolor"
            '
            'menuCommands
            '
            Me.menuCommands.Enabled = False
            Me.menuCommands.Index = 4
            Me.menuCommands.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuChannels, Me.menuEffects, Me.menuFilters, Me.menuTransforms, Me.menuDocument, Me.menuItem3, Me.menuItem16, Me.menuFlip, Me.menuItem23, Me.menuRotate, Me.menuResizeCanvas, Me.menuResample, Me.menuCrop, Me.menuAutoCrop, Me.menuSkew, Me.menuQuadrilateralWarp, Me.menuPush, Me.menuItem28, Me.menuOverlay, Me.menuItem19})
            Me.menuCommands.Text = "&Commands"
            '
            'menuChannels
            '
            Me.menuChannels.Index = 0
            Me.menuChannels.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuChannelsCombine, Me.menuChannelsReplace, Me.menuChannelsSplit, Me.menuItem13, Me.menuChannelsAdjust, Me.menuChannelsAdjustHsl, Me.menuChannelsApplyLut, Me.menuChannelsInvert, Me.menuChannelsShift, Me.menuChannelsSwap, Me.menuItem24, Me.menuChannelsFlattenAlpha, Me.menuChannelsAphaColor, Me.menuChannelsAlphaMask, Me.menuChannelsAlphaValue})
            Me.menuChannels.Text = "Channels"
            '
            'menuChannelsCombine
            '
            Me.menuChannelsCombine.Index = 0
            Me.menuChannelsCombine.Text = "Combine"
            '
            'menuChannelsReplace
            '
            Me.menuChannelsReplace.Index = 1
            Me.menuChannelsReplace.Text = "Replace"
            '
            'menuChannelsSplit
            '
            Me.menuChannelsSplit.Index = 2
            Me.menuChannelsSplit.Text = "Split"
            '
            'menuItem13
            '
            Me.menuItem13.Index = 3
            Me.menuItem13.Text = "-"
            '
            'menuChannelsAdjust
            '
            Me.menuChannelsAdjust.Index = 4
            Me.menuChannelsAdjust.Text = "Adjust"
            '
            'menuChannelsAdjustHsl
            '
            Me.menuChannelsAdjustHsl.Index = 5
            Me.menuChannelsAdjustHsl.Text = "Adjust HSL"
            '
            'menuChannelsApplyLut
            '
            Me.menuChannelsApplyLut.Index = 6
            Me.menuChannelsApplyLut.Text = "Apply LUT Demo (invert)"
            '
            'menuChannelsInvert
            '
            Me.menuChannelsInvert.Index = 7
            Me.menuChannelsInvert.Text = "Invert"
            '
            'menuChannelsShift
            '
            Me.menuChannelsShift.Index = 8
            Me.menuChannelsShift.Text = "Shift"
            '
            'menuChannelsSwap
            '
            Me.menuChannelsSwap.Index = 9
            Me.menuChannelsSwap.Text = "Swap"
            '
            'menuItem24
            '
            Me.menuItem24.Index = 10
            Me.menuItem24.Text = "-"
            '
            'menuChannelsFlattenAlpha
            '
            Me.menuChannelsFlattenAlpha.Index = 11
            Me.menuChannelsFlattenAlpha.Text = "Flatten Alpha"
            '
            'menuChannelsAphaColor
            '
            Me.menuChannelsAphaColor.Index = 12
            Me.menuChannelsAphaColor.Text = "Set Alpha By Color"
            '
            'menuChannelsAlphaMask
            '
            Me.menuChannelsAlphaMask.Index = 13
            Me.menuChannelsAlphaMask.Text = "Set Alpha From Mask"
            '
            'menuChannelsAlphaValue
            '
            Me.menuChannelsAlphaValue.Index = 14
            Me.menuChannelsAlphaValue.Text = "Set Alpha Value"
            '
            'menuEffects
            '
            Me.menuEffects.Index = 1
            Me.menuEffects.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuEffectsAdjustTint, Me.menuEffectsBevelEdge, Me.menuEffectsCrackle, Me.menuEffectsDeInterlace, Me.menuEffectsDropShadow, Me.menuEffectsFingerPrint, Me.menuEffectsFloodFill, Me.menuEffectsGamma, Me.menuEffectsGauzy, Me.menuEffectsHalftone, Me.menuEffectsMosaic, Me.menuEffectsOilPaint, Me.menuEffectsPosterize, Me.menuReduceColors, Me.menuEffectsReplaceColor, Me.menuEffectsRoundedBevel, Me.menuEffectsSaturation, Me.menuEffectsSolarize, Me.menuEffectsStipple, Me.menuEffectsTintGrayscale, Me.menuEffectsWatercolorTint, Me.menuItem46, Me.menuEffectsBrightnessHistogramEqualize, Me.menuEffectsBrightnessHistogramStretch, Me.menuHistogramEqualize, Me.menuEffectsHistogramStretch, Me.menuItem4, Me.menuEffectsRedEyeRemoval, Me.menuEffectsLevels, Me.menuEffectsAutoLevels, Me.menuEffectsAutoContrast, Me.menuEffectsAutoColor, Me.menuEffectsAutoWhiteBalance})
            Me.menuEffects.Text = "Effects"
            '
            'menuEffectsAdjustTint
            '
            Me.menuEffectsAdjustTint.Index = 0
            Me.menuEffectsAdjustTint.Text = "Adjust Tint"
            '
            'menuEffectsBevelEdge
            '
            Me.menuEffectsBevelEdge.Index = 1
            Me.menuEffectsBevelEdge.Text = "Bevel Edge"
            '
            'menuEffectsCrackle
            '
            Me.menuEffectsCrackle.Index = 2
            Me.menuEffectsCrackle.Text = "Crackle"
            '
            'menuEffectsDeInterlace
            '
            Me.menuEffectsDeInterlace.Index = 3
            Me.menuEffectsDeInterlace.Text = "De-Interlace"
            '
            'menuEffectsDropShadow
            '
            Me.menuEffectsDropShadow.Index = 4
            Me.menuEffectsDropShadow.Text = "Drop Shadow"
            '
            'menuEffectsFingerPrint
            '
            Me.menuEffectsFingerPrint.Index = 5
            Me.menuEffectsFingerPrint.Text = "Finger Print"
            '
            'menuEffectsFloodFill
            '
            Me.menuEffectsFloodFill.Index = 6
            Me.menuEffectsFloodFill.Text = "Flood Fill"
            '
            'menuEffectsGamma
            '
            Me.menuEffectsGamma.Index = 7
            Me.menuEffectsGamma.Text = "Gamma"
            '
            'menuEffectsGauzy
            '
            Me.menuEffectsGauzy.Index = 8
            Me.menuEffectsGauzy.Text = "Gauzy"
            '
            'menuEffectsHalftone
            '
            Me.menuEffectsHalftone.Index = 9
            Me.menuEffectsHalftone.Text = "Halftone"
            '
            'menuEffectsMosaic
            '
            Me.menuEffectsMosaic.Index = 10
            Me.menuEffectsMosaic.Text = "Mosaic"
            '
            'menuEffectsOilPaint
            '
            Me.menuEffectsOilPaint.Index = 11
            Me.menuEffectsOilPaint.Text = "Oil Paint"
            '
            'menuEffectsPosterize
            '
            Me.menuEffectsPosterize.Index = 12
            Me.menuEffectsPosterize.Text = "Posterize"
            '
            'menuReduceColors
            '
            Me.menuReduceColors.Index = 13
            Me.menuReduceColors.Text = "Reduce Colors"
            '
            'menuEffectsReplaceColor
            '
            Me.menuEffectsReplaceColor.Index = 14
            Me.menuEffectsReplaceColor.Text = "Replace Color"
            '
            'menuEffectsRoundedBevel
            '
            Me.menuEffectsRoundedBevel.Index = 15
            Me.menuEffectsRoundedBevel.Text = "Rounded Bevel"
            '
            'menuEffectsSaturation
            '
            Me.menuEffectsSaturation.Index = 16
            Me.menuEffectsSaturation.Text = "Saturation"
            '
            'menuEffectsSolarize
            '
            Me.menuEffectsSolarize.Index = 17
            Me.menuEffectsSolarize.Text = "Solarize"
            '
            'menuEffectsStipple
            '
            Me.menuEffectsStipple.Index = 18
            Me.menuEffectsStipple.Text = "Stipple"
            '
            'menuEffectsTintGrayscale
            '
            Me.menuEffectsTintGrayscale.Index = 19
            Me.menuEffectsTintGrayscale.Text = "Tint Grayscale"
            '
            'menuEffectsWatercolorTint
            '
            Me.menuEffectsWatercolorTint.Index = 20
            Me.menuEffectsWatercolorTint.Text = "Watercolor Tint"
            '
            'menuItem46
            '
            Me.menuItem46.Index = 21
            Me.menuItem46.Text = "-"
            '
            'menuEffectsBrightnessHistogramEqualize
            '
            Me.menuEffectsBrightnessHistogramEqualize.Index = 22
            Me.menuEffectsBrightnessHistogramEqualize.Text = "Brightness Histogram Equalize"
            '
            'menuEffectsBrightnessHistogramStretch
            '
            Me.menuEffectsBrightnessHistogramStretch.Index = 23
            Me.menuEffectsBrightnessHistogramStretch.Text = "Brightness Histogram Stretch"
            '
            'menuHistogramEqualize
            '
            Me.menuHistogramEqualize.Index = 24
            Me.menuHistogramEqualize.Text = "Histogram Equalize"
            '
            'menuEffectsHistogramStretch
            '
            Me.menuEffectsHistogramStretch.Index = 25
            Me.menuEffectsHistogramStretch.Text = "Histogram Stretch"
            '
            'menuItem4
            '
            Me.menuItem4.Index = 26
            Me.menuItem4.Text = "-"
            '
            'menuEffectsRedEyeRemoval
            '
            Me.menuEffectsRedEyeRemoval.Index = 27
            Me.menuEffectsRedEyeRemoval.Text = "Red Eye Removal"
            '
            'menuEffectsLevels
            '
            Me.menuEffectsLevels.Index = 28
            Me.menuEffectsLevels.Text = "Levels"
            '
            'menuEffectsAutoLevels
            '
            Me.menuEffectsAutoLevels.Index = 29
            Me.menuEffectsAutoLevels.Text = "Auto Levels"
            '
            'menuEffectsAutoContrast
            '
            Me.menuEffectsAutoContrast.Index = 30
            Me.menuEffectsAutoContrast.Text = "Auto Contrast"
            '
            'menuEffectsAutoColor
            '
            Me.menuEffectsAutoColor.Index = 31
            Me.menuEffectsAutoColor.Text = "Auto Color"
            '
            'menuEffectsAutoWhiteBalance
            '
            Me.menuEffectsAutoWhiteBalance.Index = 32
            Me.menuEffectsAutoWhiteBalance.Text = "Auto White Balance"
            '
            'menuFilters
            '
            Me.menuFilters.Index = 2
            Me.menuFilters.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFiltersBrightnessContrast, Me.menuFiltersSaturation, Me.menuFiltersBlur, Me.menuFiltersGaussianBlur, Me.menuAdaptiveUnsharpMask, Me.menuEffectsUnsharpMask, Me.menuFiltersSharpen, Me.menuItem58, Me.menuFiltersAddNoise, Me.menuFiltersDespeckle, Me.menuFiltersEmboss, Me.menuFiltersIntensify, Me.menuFiltersHighPass, Me.menuFiltersMaximum, Me.menuFiltersMean, Me.menuFiltersMedian, Me.menuFiltersMidpoint, Me.menuFiltersMinimum, Me.menuFiltersMorphological, Me.menuFiltersThreshold, Me.menuItem70, Me.menuFiltersConvolutionFilter, Me.menuFiltersConvolutionMatrix, Me.menuItem73, Me.menuFiltersEdge, Me.menuFiltersCannyEdgeDetector, Me.menuFiltersDustScratchRemoval})
            Me.menuFilters.Text = "Filters"
            '
            'menuFiltersBrightnessContrast
            '
            Me.menuFiltersBrightnessContrast.Index = 0
            Me.menuFiltersBrightnessContrast.Text = "Brightness and Contrast"
            '
            'menuFiltersSaturation
            '
            Me.menuFiltersSaturation.Index = 1
            Me.menuFiltersSaturation.Text = "Saturation"
            '
            'menuFiltersBlur
            '
            Me.menuFiltersBlur.Index = 2
            Me.menuFiltersBlur.Text = "Blur"
            '
            'menuFiltersGaussianBlur
            '
            Me.menuFiltersGaussianBlur.Index = 3
            Me.menuFiltersGaussianBlur.Text = "Gaussian Blur"
            '
            'menuAdaptiveUnsharpMask
            '
            Me.menuAdaptiveUnsharpMask.Index = 4
            Me.menuAdaptiveUnsharpMask.Text = "Adaptive Unsharp Mask"
            '
            'menuEffectsUnsharpMask
            '
            Me.menuEffectsUnsharpMask.Index = 5
            Me.menuEffectsUnsharpMask.Text = "Unsharp Mask"
            '
            'menuFiltersSharpen
            '
            Me.menuFiltersSharpen.Index = 6
            Me.menuFiltersSharpen.Text = "Sharpen"
            '
            'menuItem58
            '
            Me.menuItem58.Index = 7
            Me.menuItem58.Text = "-"
            '
            'menuFiltersAddNoise
            '
            Me.menuFiltersAddNoise.Index = 8
            Me.menuFiltersAddNoise.Text = "Add Noise"
            '
            'menuFiltersDespeckle
            '
            Me.menuFiltersDespeckle.Index = 9
            Me.menuFiltersDespeckle.Text = "Despeckle"
            '
            'menuFiltersEmboss
            '
            Me.menuFiltersEmboss.Index = 10
            Me.menuFiltersEmboss.Text = "Emboss"
            '
            'menuFiltersIntensify
            '
            Me.menuFiltersIntensify.Index = 11
            Me.menuFiltersIntensify.Text = "Intensify"
            '
            'menuFiltersHighPass
            '
            Me.menuFiltersHighPass.Index = 12
            Me.menuFiltersHighPass.Text = "High Pass"
            '
            'menuFiltersMaximum
            '
            Me.menuFiltersMaximum.Index = 13
            Me.menuFiltersMaximum.Text = "Maximum"
            '
            'menuFiltersMean
            '
            Me.menuFiltersMean.Index = 14
            Me.menuFiltersMean.Text = "Mean"
            '
            'menuFiltersMedian
            '
            Me.menuFiltersMedian.Index = 15
            Me.menuFiltersMedian.Text = "Median"
            '
            'menuFiltersMidpoint
            '
            Me.menuFiltersMidpoint.Index = 16
            Me.menuFiltersMidpoint.Text = "Midpoint"
            '
            'menuFiltersMinimum
            '
            Me.menuFiltersMinimum.Index = 17
            Me.menuFiltersMinimum.Text = "Minimum"
            '
            'menuFiltersMorphological
            '
            Me.menuFiltersMorphological.Index = 18
            Me.menuFiltersMorphological.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuMorphoDilation, Me.menuMorphoErosion, Me.menuMorphoOpen, Me.menuMorphoClose, Me.menuMorphoTophat, Me.menuMorphoGradient})
            Me.menuFiltersMorphological.Text = "Morphological"
            '
            'menuMorphoDilation
            '
            Me.menuMorphoDilation.Index = 0
            Me.menuMorphoDilation.Text = "Dilation"
            '
            'menuMorphoErosion
            '
            Me.menuMorphoErosion.Index = 1
            Me.menuMorphoErosion.Text = "Erosion"
            '
            'menuMorphoOpen
            '
            Me.menuMorphoOpen.Index = 2
            Me.menuMorphoOpen.Text = "Open"
            '
            'menuMorphoClose
            '
            Me.menuMorphoClose.Index = 3
            Me.menuMorphoClose.Text = "Close"
            '
            'menuMorphoTophat
            '
            Me.menuMorphoTophat.Index = 4
            Me.menuMorphoTophat.Text = "Tophat"
            '
            'menuMorphoGradient
            '
            Me.menuMorphoGradient.Index = 5
            Me.menuMorphoGradient.Text = "Gradient"
            '
            'menuFiltersThreshold
            '
            Me.menuFiltersThreshold.Index = 19
            Me.menuFiltersThreshold.Text = "Threshold"
            '
            'menuItem70
            '
            Me.menuItem70.Index = 20
            Me.menuItem70.Text = "-"
            '
            'menuFiltersConvolutionFilter
            '
            Me.menuFiltersConvolutionFilter.Index = 21
            Me.menuFiltersConvolutionFilter.Text = "Convolution Filter"
            '
            'menuFiltersConvolutionMatrix
            '
            Me.menuFiltersConvolutionMatrix.Index = 22
            Me.menuFiltersConvolutionMatrix.Text = "Convolution Matrix"
            '
            'menuItem73
            '
            Me.menuItem73.Index = 23
            Me.menuItem73.Text = "-"
            '
            'menuFiltersEdge
            '
            Me.menuFiltersEdge.Index = 24
            Me.menuFiltersEdge.Text = "Edge Detection"
            '
            'menuFiltersCannyEdgeDetector
            '
            Me.menuFiltersCannyEdgeDetector.Index = 25
            Me.menuFiltersCannyEdgeDetector.Text = "Canny Edge Detector"
            '
            'menuFiltersDustScratchRemoval
            '
            Me.menuFiltersDustScratchRemoval.Index = 26
            Me.menuFiltersDustScratchRemoval.Text = "Dust && Scratch Removal"
            '
            'menuTransforms
            '
            Me.menuTransforms.Index = 3
            Me.menuTransforms.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuTransformsChain, Me.menuTransformsApply, Me.menuItem77, Me.menuTransformsBumpMap, Me.menuTransformsElliptical, Me.menuTransformsLens, Me.menuLineSlice, Me.menuTransformsMarble, Me.menuTransformsOffset, Me.menuTransformsPerlin, Me.menuTransformsPinch, Me.menuTransformsPolygon, Me.menuTransformsRandom, Me.menuTransformsRipple, Me.menuTransformsSpin, Me.menuTransformSpinWave, Me.menuTransformsWave, Me.menuTransformsWow, Me.menuTransformsZigZag, Me.menuItem94, Me.menuTransformsUser})
            Me.menuTransforms.Text = "Transforms"
            '
            'menuTransformsChain
            '
            Me.menuTransformsChain.Index = 0
            Me.menuTransformsChain.Text = "Chain Transforms Together"
            '
            'menuTransformsApply
            '
            Me.menuTransformsApply.Enabled = False
            Me.menuTransformsApply.Index = 1
            Me.menuTransformsApply.Text = "Apply Transformation Chain"
            '
            'menuItem77
            '
            Me.menuItem77.Index = 2
            Me.menuItem77.Text = "-"
            '
            'menuTransformsBumpMap
            '
            Me.menuTransformsBumpMap.Index = 3
            Me.menuTransformsBumpMap.Text = "Bump Map"
            '
            'menuTransformsElliptical
            '
            Me.menuTransformsElliptical.Index = 4
            Me.menuTransformsElliptical.Text = "Elliptical"
            '
            'menuTransformsLens
            '
            Me.menuTransformsLens.Index = 5
            Me.menuTransformsLens.Text = "Lens"
            '
            'menuLineSlice
            '
            Me.menuLineSlice.Index = 6
            Me.menuLineSlice.Text = "Line Slice"
            '
            'menuTransformsMarble
            '
            Me.menuTransformsMarble.Index = 7
            Me.menuTransformsMarble.Text = "Marble"
            '
            'menuTransformsOffset
            '
            Me.menuTransformsOffset.Index = 8
            Me.menuTransformsOffset.Text = "Offset"
            '
            'menuTransformsPerlin
            '
            Me.menuTransformsPerlin.Index = 9
            Me.menuTransformsPerlin.Text = "Perlin"
            '
            'menuTransformsPinch
            '
            Me.menuTransformsPinch.Index = 10
            Me.menuTransformsPinch.Text = "Pinch"
            '
            'menuTransformsPolygon
            '
            Me.menuTransformsPolygon.Index = 11
            Me.menuTransformsPolygon.Text = "Polygon"
            '
            'menuTransformsRandom
            '
            Me.menuTransformsRandom.Index = 12
            Me.menuTransformsRandom.Text = "Random"
            '
            'menuTransformsRipple
            '
            Me.menuTransformsRipple.Index = 13
            Me.menuTransformsRipple.Text = "Ripple"
            '
            'menuTransformsSpin
            '
            Me.menuTransformsSpin.Index = 14
            Me.menuTransformsSpin.Text = "Spin"
            '
            'menuTransformSpinWave
            '
            Me.menuTransformSpinWave.Index = 15
            Me.menuTransformSpinWave.Text = "Spin Wave"
            '
            'menuTransformsWave
            '
            Me.menuTransformsWave.Index = 16
            Me.menuTransformsWave.Text = "Wave"
            '
            'menuTransformsWow
            '
            Me.menuTransformsWow.Index = 17
            Me.menuTransformsWow.Text = "Wow"
            '
            'menuTransformsZigZag
            '
            Me.menuTransformsZigZag.Index = 18
            Me.menuTransformsZigZag.Text = "Zig Zag"
            '
            'menuItem94
            '
            Me.menuItem94.Index = 19
            Me.menuItem94.Text = "-"
            '
            'menuTransformsUser
            '
            Me.menuTransformsUser.Index = 20
            Me.menuTransformsUser.Text = "User Transform Demo"
            '
            'menuDocument
            '
            Me.menuDocument.Index = 4
            Me.menuDocument.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuDocumentAutoDeskew, Me.menuDocumentMedian, Me.menuDocumentDespeckle, Me.menuItem103, Me.menuDocumentMorphological, Me.menuDocumentThinning, Me.menuDocumentHitOrMiss, Me.menuThresholding, Me.menuBorderRemoval, Me.menuDithering})
            Me.menuDocument.Text = "Document"
            '
            'menuDocumentAutoDeskew
            '
            Me.menuDocumentAutoDeskew.Index = 0
            Me.menuDocumentAutoDeskew.Text = "Auto Deskew"
            '
            'menuDocumentMedian
            '
            Me.menuDocumentMedian.Index = 1
            Me.menuDocumentMedian.Text = "Remove Noise (Median)"
            '
            'menuDocumentDespeckle
            '
            Me.menuDocumentDespeckle.Index = 2
            Me.menuDocumentDespeckle.Text = "Despeckle"
            '
            'menuItem103
            '
            Me.menuItem103.Index = 3
            Me.menuItem103.Text = "-"
            '
            'menuDocumentMorphological
            '
            Me.menuDocumentMorphological.Index = 4
            Me.menuDocumentMorphological.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuBinaryDilation, Me.menuBinaryErosion, Me.menuBinaryOpen, Me.menuBinaryClose, Me.menuBinaryBoundary})
            Me.menuDocumentMorphological.Text = "Morphological"
            '
            'menuBinaryDilation
            '
            Me.menuBinaryDilation.Index = 0
            Me.menuBinaryDilation.Text = "Dilation"
            '
            'menuBinaryErosion
            '
            Me.menuBinaryErosion.Index = 1
            Me.menuBinaryErosion.Text = "Erosion"
            '
            'menuBinaryOpen
            '
            Me.menuBinaryOpen.Index = 2
            Me.menuBinaryOpen.Text = "Open"
            '
            'menuBinaryClose
            '
            Me.menuBinaryClose.Index = 3
            Me.menuBinaryClose.Text = "Close"
            '
            'menuBinaryBoundary
            '
            Me.menuBinaryBoundary.Index = 4
            Me.menuBinaryBoundary.Text = "Boundary Extraction"
            '
            'menuDocumentThinning
            '
            Me.menuDocumentThinning.Index = 5
            Me.menuDocumentThinning.Text = "Thinning"
            '
            'menuDocumentHitOrMiss
            '
            Me.menuDocumentHitOrMiss.Index = 6
            Me.menuDocumentHitOrMiss.Text = "Hit or Miss"
            '
            'menuThresholding
            '
            Me.menuThresholding.Index = 7
            Me.menuThresholding.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuThresholdAdaptive, Me.menuThresholdGlobal, Me.menuThresholdDynamic})
            Me.menuThresholding.Text = "Thresholding"
            '
            'menuThresholdAdaptive
            '
            Me.menuThresholdAdaptive.Index = 0
            Me.menuThresholdAdaptive.Text = "Adaptive"
            '
            'menuThresholdGlobal
            '
            Me.menuThresholdGlobal.Index = 1
            Me.menuThresholdGlobal.Text = "Global"
            '
            'menuThresholdDynamic
            '
            Me.menuThresholdDynamic.Index = 2
            Me.menuThresholdDynamic.Text = "Dynamic"
            '
            'menuBorderRemoval
            '
            Me.menuBorderRemoval.Index = 8
            Me.menuBorderRemoval.Text = "Border Removal"
            '
            'menuItem3
            '
            Me.menuItem3.Index = 5
            Me.menuItem3.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFftBandPass, Me.menuFftButterworthHighBoost, Me.menuFftButterworthHighPass, Me.menuFftButterworthLowPass, Me.menuFftGaussianHighBoost, Me.menuFftGaussianHighPass, Me.menuFftGaussianLowPass, Me.menuFftIdealHighPass, Me.menuFftIdealLowPass, Me.menuFftInversePower})
            Me.menuItem3.Text = "FFT"
            '
            'menuFftBandPass
            '
            Me.menuFftBandPass.Index = 0
            Me.menuFftBandPass.Text = "Band Pass Filter"
            '
            'menuFftButterworthHighBoost
            '
            Me.menuFftButterworthHighBoost.Index = 1
            Me.menuFftButterworthHighBoost.Text = "Butterworth High Boost"
            '
            'menuFftButterworthHighPass
            '
            Me.menuFftButterworthHighPass.Index = 2
            Me.menuFftButterworthHighPass.Text = "Butterworth High Pass"
            '
            'menuFftButterworthLowPass
            '
            Me.menuFftButterworthLowPass.Index = 3
            Me.menuFftButterworthLowPass.Text = "Butterworth Low Pass "
            '
            'menuFftGaussianHighBoost
            '
            Me.menuFftGaussianHighBoost.Index = 4
            Me.menuFftGaussianHighBoost.Text = "Gaussian High Boost"
            '
            'menuFftGaussianHighPass
            '
            Me.menuFftGaussianHighPass.Index = 5
            Me.menuFftGaussianHighPass.Text = "Gaussian High Pass"
            '
            'menuFftGaussianLowPass
            '
            Me.menuFftGaussianLowPass.Index = 6
            Me.menuFftGaussianLowPass.Text = "Gaussian Low Pass"
            '
            'menuFftIdealHighPass
            '
            Me.menuFftIdealHighPass.Index = 7
            Me.menuFftIdealHighPass.Text = "Ideal High Pass"
            '
            'menuFftIdealLowPass
            '
            Me.menuFftIdealLowPass.Index = 8
            Me.menuFftIdealLowPass.Text = "Ideal Low Pass"
            '
            'menuFftInversePower
            '
            Me.menuFftInversePower.Index = 9
            Me.menuFftInversePower.Text = "Inverse Power"
            '
            'menuItem16
            '
            Me.menuItem16.Index = 6
            Me.menuItem16.Text = "-"
            '
            'menuFlip
            '
            Me.menuFlip.Index = 7
            Me.menuFlip.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuFlipHorizontal, Me.menuFlipVertical})
            Me.menuFlip.Text = "Flip"
            '
            'menuFlipHorizontal
            '
            Me.menuFlipHorizontal.Index = 0
            Me.menuFlipHorizontal.Text = "Horizontal"
            '
            'menuFlipVertical
            '
            Me.menuFlipVertical.Index = 1
            Me.menuFlipVertical.Text = "Vertical"
            '
            'menuItem23
            '
            Me.menuItem23.Index = 8
            Me.menuItem23.Text = "-"
            '
            'menuRotate
            '
            Me.menuRotate.Index = 9
            Me.menuRotate.Text = "Rotate"
            '
            'menuResizeCanvas
            '
            Me.menuResizeCanvas.Index = 10
            Me.menuResizeCanvas.Text = "Resize Canvas"
            '
            'menuResample
            '
            Me.menuResample.Index = 11
            Me.menuResample.Text = "Resample"
            '
            'menuCrop
            '
            Me.menuCrop.Index = 12
            Me.menuCrop.Text = "Crop"
            '
            'menuAutoCrop
            '
            Me.menuAutoCrop.Index = 13
            Me.menuAutoCrop.Text = "Auto Crop"
            '
            'menuSkew
            '
            Me.menuSkew.Index = 14
            Me.menuSkew.Text = "Skew"
            '
            'menuQuadrilateralWarp
            '
            Me.menuQuadrilateralWarp.Index = 15
            Me.menuQuadrilateralWarp.Text = "Quadrilateral Warp"
            '
            'menuPush
            '
            Me.menuPush.Index = 16
            Me.menuPush.Text = "Push"
            '
            'menuItem28
            '
            Me.menuItem28.Index = 17
            Me.menuItem28.Text = "-"
            '
            'menuOverlay
            '
            Me.menuOverlay.Index = 18
            Me.menuOverlay.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuOverlayNormal, Me.menuOverlayMasked, Me.menuOverlayMerged})
            Me.menuOverlay.Text = "Overlay"
            '
            'menuOverlayNormal
            '
            Me.menuOverlayNormal.Index = 0
            Me.menuOverlayNormal.Text = "Normal"
            '
            'menuOverlayMasked
            '
            Me.menuOverlayMasked.Index = 1
            Me.menuOverlayMasked.Text = "Masked"
            '
            'menuOverlayMerged
            '
            Me.menuOverlayMerged.Index = 2
            Me.menuOverlayMerged.Text = "Merge Options"
            '
            'menuItem19
            '
            Me.menuItem19.Index = 19
            Me.menuItem19.Text = "-"
            '
            'menuHelp
            '
            Me.menuHelp.Index = 5
            Me.menuHelp.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuItem15})
            Me.menuHelp.Text = "&Help"
            '
            'menuItem15
            '
            Me.menuItem15.Index = 0
            Me.menuItem15.Text = "About ..."
            '
            'cboFrameIndex
            '
            Me.cboFrameIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboFrameIndex.Location = New System.Drawing.Point(284, 2)
            Me.cboFrameIndex.Name = "cboFrameIndex"
            Me.cboFrameIndex.Size = New System.Drawing.Size(132, 21)
            Me.cboFrameIndex.TabIndex = 6
            '
            'menuDithering
            '
            Me.menuDithering.Index = 9
            Me.menuDithering.Text = "Dithering"
            '
            'FormMain
            '
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.ClientSize = New System.Drawing.Size(552, 358)
            Me.Controls.Add(Me.cboFrameIndex)
            Me.Controls.Add(Me.Viewer)
            Me.Controls.Add(Me.progressBar1)
            Me.Controls.Add(Me.toolBar1)
            Me.Controls.Add(Me.statusInfo)
            Me.Menu = Me.mainMenu
            Me.Name = "FormMain"
            Me.Text = "Atalasoft dotImage Demo"
            CType(Me.statusBarPosition, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.statusBarLoadTime, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.statusBarMessage, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.statusBarProgress, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub


        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        <STAThread()> _
        Shared Sub Main()
            Application.Run(New FormMain)
        End Sub
#End Region


#Region "Viewer Events"

        Private Sub Viewer_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Viewer.MouseDown
            If e.Button = MouseButtons.Right Then
                If Viewer.Selection.Active Then
                    Viewer.Selection.Visible = False
                End If
                Me.lineDraw.Cancel()
                Me.rectangleDraw.Cancel()
                Me.ellipseDraw.Cancel()

                If Me.drawMode = DrawMenuMode.Polygon Then
                    ' We undo so the lines will not draw over eachother.
                    ' Another option is to simply draw the last connection line,
                    ' but then we couldn't fill the polygon directly.
                    Me.Viewer.Undos.Undo()
                    Viewer.Undos.Add("Draw Polygon", True)

                    Dim myCanvas As Canvas = New Canvas(Me.Viewer.Image)
                    If Not Me.polygonPoints Is Nothing AndAlso Me.polygonPoints.Length > 1 Then
                        myCanvas.DrawPolygon(Me.polygonPoints, Me.lineDraw.Pen, Me.rectangleDraw.Fill)
                    End If
                    Me.polygonPoints = Nothing
                    Me.Viewer.Refresh()
                End If

                Me.drawMode = DrawMenuMode.None
                Me.lineDraw.Active = False
                Me.rectangleDraw.Active = False
                Me.ellipseDraw.Active = False
            ElseIf (e.Button = MouseButtons.Left) AndAlso (Me.drawMode = DrawMenuMode.Freehand) Then
                'start the freehand drawing
                Me.polygonPoints = New Point(0) {New Point(CInt((e.X / Viewer.Zoom) - (Viewer.ImagePosition.X / Viewer.Zoom)), CInt((e.Y / Viewer.Zoom) - (Viewer.ImagePosition.Y / Viewer.Zoom)))}
            End If
        End Sub

        Private Sub Viewer_MouseMovePixel(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Viewer.MouseMovePixel
            If Viewer.Selection.Visible Then
                Dim rc As Rectangle = Viewer.Selection.Bounds
                statusBarPosition.Text = "Selection:  " & rc.Left & ", " & rc.Top & ", " & rc.Width & ", " & rc.Height
            Else
                Dim pos As String = "Position:  " & e.X.ToString() & " x " & e.Y.ToString()
                statusBarPosition.Text = pos
            End If
            statusBarPosition.ToolTipText = statusBarPosition.Text

            If (Me.drawMode = DrawMenuMode.Freehand) AndAlso (e.Button = MouseButtons.Left) Then
                Dim myCanvas As Canvas = New Canvas(Viewer.Image)
                myCanvas.DrawLine(Me.polygonPoints(0), New Point(e.X, e.Y), Me.lineDraw.Pen)
                Me.polygonPoints(0) = New Point(e.X, e.Y)
                Viewer.Refresh()
            End If
        End Sub

        Private Sub Viewer_Progress(ByVal sender As Object, ByVal e As ProgressEventArgs) Handles Viewer.Progress
            If e.Total = 0 Then
                e.Total = 1
            End If
            progressBar1.Value = e.Current * 100 / e.Total
            If progressBar1.Value = 100 Then
                progressBar1.Value = 0
            End If

        End Sub

        'handle this event to close the memory stream when complete when opening and saving to/from memory
        Private Sub Viewer_ImageStreamCompleted(ByVal sender As Object, ByVal e As Atalasoft.Imaging.ImageStreamEventArgs) Handles Viewer.ImageStreamCompleted
            e.Stream.Close()
        End Sub

        Private Sub Viewer_ProcessError(ByVal sender As Object, ByVal e As Atalasoft.Imaging.ExceptionEventArgs) Handles Viewer.ProcessError
            MessageBox.Show(Me, e.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.isProcessError = True
        End Sub

        Private Sub Viewer_ChangedImage(ByVal sender As Object, ByVal e As Atalasoft.Imaging.ImageEventArgs) Handles Viewer.ImageChanged
            If e.Image Is Nothing Then
                Return
            End If
            statusBarMessage.Text = e.Image.ToString()
            Viewer.Update()

            EnableMenuItems()
            If Not e.Image Is Nothing Then
                statusBarMessage.Text = e.Image.ToString()
            End If

            UpdateUndoRedoInfo()

            If Not Me.histogram Is Nothing Then
                Me.histogram.SetHistogram(e.Image)
            End If
        End Sub

        Private Sub Viewer_ProcessCompleted(ByVal sender As Object, ByVal e As ImageEventArgs) Handles Viewer.ProcessCompleted
            If Me.isProcessError Then
                Me.isProcessError = False
                Return
            End If
            If Me.performingOpen Then
                DisplayLoadTime()
                Me.performingOpen = False
                ReadMetadata()
            End If
        End Sub

        Private Sub Viewer_MouseWheel(ByVal sender As Object, ByVal e As MouseEventArgs)
            If Me.Viewer.Image Is Nothing Then
                Return
            End If

            Dim pt As Point = Me.Viewer.ScrollPosition
            Dim jump As Integer = (Me.Viewer.ClientSize.Height / 10)

            If e.Delta < 0 Then
                pt.Y -= jump
            Else
                pt.Y += jump
            End If

            If pt.Y > 0 Then
                pt.Y = 0
            End If
            Me.Viewer.ScrollPosition = pt
        End Sub

        Private Sub AtalaImage_ChangePixelFormat(ByVal sender As Object, ByVal e As PixelFormatChangeEventArgs)
            If Me.InvokeRequired Then
                Me.Invoke(New PixelFormatChangeEventHandler(AddressOf AtalaImage_ChangePixelFormat), New Object() {sender, e})
            Else
                ' Disable this dialog if using the magnifier with 4-bit images.
                If Me.Viewer.Magnifier.Active AndAlso Me.Viewer.Image.PixelFormat = PixelFormat.Pixel4bppIndexed Then
                    Return
                End If

                ' Ask if it's ok to change the pixel format.
                e.Cancel = CBool(MessageBox.Show(Me, "This action requires the pixel format to be changed:" & Constants.vbLf + Constants.vbLf & "Current: " & Constants.vbTab + e.CurrentPixelFormat.ToString() & Constants.vbLf & "New: " & Constants.vbTab + e.NewPixelFormat.ToString() & Constants.vbLf + Constants.vbLf & "Do you want to continue?", "Change Pixel Format", MessageBoxButtons.YesNo) = DialogResult.No)
            End If
        End Sub

#End Region

#Region "Form Events"

        Private Sub toolBar1_ButtonClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolBarButtonClickEventArgs) Handles toolBar1.ButtonClick
            Select Case e.Button.ToolTipText
                Case "Open"
                    OpenAndLoadImage()

                Case "Save"
                    SaveCurrentImage(Nothing)

                Case "Undo"
                    Viewer.Undos.Undo()
                    UpdateUndoRedoInfo()

                Case "Redo"
                    Viewer.Undos.Redo()
                    UpdateUndoRedoInfo()

                Case "Arrow"
                    ClearMouseTools()
                    tbArrow.Pushed = True
                Case "Rectangle Selection"
                    ClearMouseTools()
                    tbSelectRectangle.Pushed = True
                    Viewer.MouseTool = MouseToolType.Selection
                    Viewer.Selection = Me.rectangleSelection
                Case "Ellipse Selection"
                    ClearMouseTools()
                    tbSelectEllipse.Pushed = True
                    Viewer.MouseTool = MouseToolType.Selection
                    Viewer.Selection = Me.ellipseRubberband
                Case "Pan"
                    ClearMouseTools()
                    tbPan.Pushed = True
                    Viewer.MouseTool = MouseToolType.Pan
                Case "Magnifier"
                    ClearMouseTools()
                    tbMagnifier.Pushed = True
                    Viewer.MouseTool = MouseToolType.Magnifier
                Case "Zoom"
                    ClearMouseTools()
                    tbZoom.Pushed = True
                    Viewer.MouseTool = MouseToolType.Zoom
                Case "Zoom Selection"
                    ClearMouseTools()
                    tbZoomSelection.Pushed = True
                    Viewer.MouseTool = MouseToolType.ZoomArea
            End Select
        End Sub

        Private Sub cboFrameIndex_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFrameIndex.SelectedIndexChanged
            If Me.cboFrameIndex.SelectedIndex = _currentIndex Then
                Return
            End If
            _currentIndex = Me.cboFrameIndex.SelectedIndex

            Dim info As ImageFileLoadData = CType(_images(_currentIndex), ImageFileLoadData)
            Try
                Me.Cursor = Cursors.WaitCursor
                Dim img As AtalaImage = New AtalaImage(info.FileName, info.FrameIndex, Nothing)
                Viewer.Images.Clear()
                Viewer.Images.Add(img)
            Catch ex As System.Exception
                MessageBox.Show(Me, ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                Me.Cursor = Cursors.Default
            End Try
        End Sub

#End Region

#Region "Private Methods"
        ' helper used to clear all of the mouse tools
        Private Sub ClearMouseTools()
            Viewer.MouseTool = MouseToolType.None
            tbSelectRectangle.Pushed = False
            tbSelectEllipse.Pushed = False
            tbPan.Pushed = False
            tbArrow.Pushed = False
            tbMagnifier.Pushed = False
            tbZoom.Pushed = False
            tbZoomSelection.Pushed = False
            tbArrow.Pushed = True

            Me.lineDraw.Active = False
            Me.rectangleDraw.Active = False
            Me.ellipseDraw.Active = False
        End Sub

        'used for most commands to show the Property Grid Editor and apply the command to the Workspace
        Public Sub ShowCommand(ByVal caption As String, ByVal command As ImageCommand)
            ' Display the parameter for this command.
            Dim rCommand As ImageRegionCommand = CType(IIf(TypeOf command Is ImageRegionCommand, command, Nothing), ImageRegionCommand)
            If Not rCommand Is Nothing Then
                rCommand.RegionOfInterest = Me.GetRegionOfInterest()
            End If
            Dim frm As Parameters = New Parameters(caption, command)
            If frm.ShowDialog() = DialogResult.OK Then
                Viewer.ApplyCommand(command, caption)
                UpdateUndoRedoInfo()
            End If
        End Sub

        'used for transform commands to add the command to the transform chain, then show the editor
        Public Sub ShowTransformCommand(ByVal caption As String, ByVal command As Transform)

            ' Handle Transform chains.
            If Me.transformChain Is Nothing Then
                transformChain = New TransformChainCommand
            End If

            If (Not Me.chainTransforms) Then
                ShowCommand(caption, command)
            Else
                Dim frm As Parameters = New Parameters(caption, command)
                If frm.ShowDialog() = DialogResult.OK Then
                    transformChain.Add(command)
                End If

            End If
        End Sub

        'returns the region of interest occupied by the selection
        Private Function GetRegionOfInterest() As RegionOfInterest
            If Viewer.Selection.Visible Then
                Return New RegionOfInterest(Viewer.Selection.GetRegion())
            Else
                Return Nothing
            End If
        End Function

        ' Updates the Undo and Redo menu and toolbar items.
        Private Sub UpdateUndoRedoInfo()
            Me.Cursor = Cursors.WaitCursor

            ' Update the undo menu and toolbar item.
            Dim item As MenuItem = Me.menuEditUndo
            If Not item Is Nothing Then
                item.Enabled = CBool(Viewer.Undos.NumUndos > 0)
                If item.Enabled Then
                    item.Text = ("&Undo " & Viewer.Undos(Viewer.Undos.Count - Viewer.Undos.NumUndos).Description)
                Else
                    item.Text = ("&Undo")
                End If
                tbUndo.Enabled = item.Enabled
            End If

            ' Update the redo menu and toolbar item.
            item = Me.menuEditRedo
            If Not item Is Nothing Then
                item.Enabled = CBool(Viewer.Undos.NumRedos > 0)
                If item.Enabled Then
                    item.Text = ("&Redo " & Viewer.Undos(Viewer.Undos.Count - Viewer.Undos.NumUndos - 1).Description)
                Else
                    item.Text = ("&Redo")
                End If
                tbRedo.Enabled = item.Enabled
            End If

            ' Update the Frame Index combobox.
            If Me.cboFrameIndex.Items.Count <> _images.Count Then
                Me.cboFrameIndex.Items.Clear()

                Dim c As Integer = _images.Count
                Dim i As Integer = 0
                'ORIGINAL LINE: for (int i = 0; i < c; i += 1)
                'INSTANT VB NOTE: This 'for' loop was translated to a VB 'Do While' loop:
                Do While i < c
                    Me.cboFrameIndex.Items.Add("Frame Number " & i.ToString())
                    If i = _currentIndex Then
                        Me.cboFrameIndex.SelectedIndex = i
                    End If
                    i += 1
                Loop
            ElseIf _images.Count > 0 Then
                Me.cboFrameIndex.SelectedIndex = _currentIndex
            End If

            ' Update the statusbar.
            If Not Viewer.Image Is Nothing Then
                statusBarMessage.Text = Viewer.Image.ToString()
            Else
                statusBarMessage.Text = "No Image"
            End If

            Me.Cursor = Cursors.Default
        End Sub

        ' This is called during the user transform process.
        Private Function UserTransformPixel(ByVal data As UserTransformData) As Boolean
            ' This will move the image to the right."
            data.FromPixel = New PointF(CSng(data.CurrentPixel.X - 10.5), CSng(data.CurrentPixel.Y))

            ' Return true to continue.
            Return True
        End Function

        Private Sub OpenAndLoadImage()
            Dim openFile As OpenFileDialog = New OpenFileDialog
            openFile.Filter = HelperMethods.CreateDialogFilter(True)

            If _firstLoad Then
                ' try to locate images folder
                Dim imagesFolder As String = Application.ExecutablePath
                ' we assume we are running under the DotImage install folder
                Dim pos As Integer = imagesFolder.IndexOf("DotImage ")
                If pos <> -1 Then
                    imagesFolder = imagesFolder.Substring(0, imagesFolder.IndexOf("\", pos)) & "\Images"
                End If

                'use this folder as starting point			
                openFile.InitialDirectory = imagesFolder
            End If

            If openFile.ShowDialog() = DialogResult.OK Then
                Try
                    Me.currentFile = openFile.FileName
                    Me.performingOpen = True

                    _startTick = System.Environment.TickCount
                    Dim fs As FileStream = New FileStream(Me.currentFile, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Dim dec As ImageDecoder = RegisteredDecoders.GetDecoder(fs)
                    fs.Seek(0, SeekOrigin.Begin)

                    Dim frm As Parameters = New Parameters("Open Image", dec)

                    If frm.ShowDialog() = DialogResult.OK Then
                        Me.Cursor = Cursors.WaitCursor

                        ' This demo only keeps one image loaded at a time.
                        _images.Clear()
                        _currentIndex = 0
                        Viewer.Images.Clear()

                        Dim frameCount As Integer = dec.GetImageInfo(fs).FrameCount
                        fs.Seek(0, SeekOrigin.Begin)

                        Dim i As Integer = 0
                        'ORIGINAL LINE: for (int i = 0; i < frameCount; i += 1)
                        'INSTANT VB NOTE: This 'for' loop was translated to a VB 'Do While' loop:
                        Do While i < frameCount
                            _images.Add(New ImageFileLoadData(openFile.FileName, i))
                            i += 1
                        Loop

                        Viewer.Open(fs, 0)
                    End If
                    ' If it's not asynchronous, get the metadata here.
                    If (Not Me.Viewer.Asynchronous) Then
                        DisplayLoadTime()
                        Me.performingOpen = False
                        ReadMetadata()
                    End If
                Catch ex As System.Exception
                    ' Show the exception.
                    MessageBox.Show(Me, ex.Message)
                    Me.performingOpen = False
                Finally
                    Me.Cursor = Cursors.Default
                End Try

                If Not openFile Is Nothing Then
                    openFile.Dispose()
                End If
            End If
        End Sub

        Public Sub DisplayLoadTime()
            Dim time As Double = (CDbl(System.Environment.TickCount) - _startTick) / 1000
            Dim fs As FileStream = File.OpenRead(Me.currentFile)
            Dim dec As ImageDecoder = RegisteredDecoders.GetDecoder(fs)
            fs.Close()
            Me.statusBarLoadTime.Text = time.ToString("0.##") & " sec with " & dec.GetType().Name
        End Sub

        'used prior to saving an image
        Private Sub PrepareMetadataToSave(ByVal encoder As ImageEncoder)
            If TypeOf encoder Is JpegEncoder Then
                Dim jpg As JpegEncoder = CType(encoder, JpegEncoder)
                jpg.IptcTags = Me.iptcItems
                jpg.ComText = Me.comItems
                jpg.AppMarkers = Me.jpegMarkers
            ElseIf TypeOf encoder Is PngEncoder Then
                Dim png As PngEncoder = CType(encoder, PngEncoder)
                png.ComText = Me.comItems
            ElseIf TypeOf encoder Is TiffEncoder Then
                Dim tif As TiffEncoder = CType(encoder, TiffEncoder)
                tif.IptcTags = Me.iptcItems
            End If

        End Sub

        Private Sub SaveCurrentImage(ByVal url As UrlParameter)

            Try
                'show the save dialog
                Dim saveFile As SaveFileDialog = New SaveFileDialog
                saveFile.Filter = HelperMethods.CreateDialogFilter(False)

                If Me.currentFile.Length > 3 Then
                    saveFile.DefaultExt = Me.currentFile.Substring(Me.currentFile.Length - 3)
                End If

                Dim encoder As ImageEncoder = Nothing
                If url Is Nothing Then
                    If saveFile.ShowDialog() = DialogResult.OK Then
                        Me.currentFile = saveFile.FileName
                        encoder = HelperMethods.GetImageEncoder(saveFile.FilterIndex)
                    Else
                        saveFile.Dispose()
                        Return
                    End If
                Else
                    Select Case url.FileFormat
                        Case ImageFileFormats.Bmp
                            encoder = New BmpEncoder
                        Case ImageFileFormats.Gif
                            encoder = New GifEncoder
                        Case ImageFileFormats.Jpeg
                            encoder = New JpegEncoder
                        Case ImageFileFormats.Png
                            encoder = New PngEncoder
                    End Select
                End If

                ' Custom saving code for animated GIF, TIFF and PDF
                If encoder Is Nothing AndAlso saveFile.FileName.EndsWith(".gif") Then
                    SaveAnimatedGif()
                ElseIf TypeOf encoder Is PdfEncoder Then
                    encoder = Nothing
                    SavePdf()
                End If

                saveFile.Dispose()

                If encoder Is Nothing Then
                    Return
                End If

                ' Save the metadata with the image
                Me.PrepareMetadataToSave(encoder)

                'show encoder properties and save
                Dim saveFrm As Form = New Parameters("Encoder Settings", encoder)
                If saveFrm.ShowDialog() = DialogResult.OK Then
                    ' Save it.
                    Dim file As String
                    If url Is Nothing Then
                        file = (Me.currentFile)
                    Else
                        file = (url.ToString())
                    End If
                    If TypeOf encoder Is TiffEncoder AndAlso _images.Count > 1 Then
                        SaveMultiPageTiff(file, CType(encoder, TiffEncoder))
                    Else
                        Viewer.Save(file, encoder)
                    End If
                End If
                saveFrm.Dispose()
            Catch ex As System.Exception
                MessageBox.Show(Me, ex.Message)
            End Try
        End Sub

        Private Sub EnableMenuItems()
            Me.tbSave.Enabled = True
            Me.menuFileSaveAs.Enabled = True
            Me.menuFileSaveFTP.Enabled = True
            Me.menuEditCut.Enabled = True
            Me.menuEditCopy.Enabled = True
            Me.menuEditPaste.Enabled = True
            Me.menuImage.Enabled = True
            Me.menuDraw.Enabled = True
            Me.menuCommands.Enabled = True
            Me.menuFilePageSetup.Enabled = True
            Me.menuFilePrintImage.Enabled = True

            Me.menuImageExif.Enabled = (Not Me.exifItems Is Nothing AndAlso Me.exifItems.Count > 0)
            Me.menuImageIptc.Enabled = (Not Me.iptcItems Is Nothing AndAlso Me.iptcItems.Count > 0)
        End Sub

        Private Sub SaveMultiPageTiff(ByVal filename As String, ByVal encoder As TiffEncoder)
            ' This method is required because we only keep one image in memory.
            'INSTANT VB NOTE: The following 'using' block is replaced by its pre-VB.NET 2005 equivalent:
            '			using (FileStream fs = New FileStream(file, (encoder.Append && File.Exists(file) ? FileMode.Open : FileMode.Create), FileAccess.ReadWrite))
            Dim fs As FileStream
            fs = New FileStream(filename, (IIf(encoder.Append AndAlso File.Exists(filename), FileMode.Open, FileMode.Create)), FileAccess.ReadWrite)
            Try
                Dim i As Integer = 0
                'ORIGINAL LINE: for (int i = 0; i < _images.Count; i += 1)
                'INSTANT VB NOTE: This 'for' loop was translated to a VB 'Do While' loop:
                Do While i < _images.Count
                    Dim data As ImageFileLoadData = CType(_images(i), ImageFileLoadData)
                    Dim image As AtalaImage
                    If i = _currentIndex Then
                        image = (Viewer.Image)
                    Else
                        image = (New AtalaImage(data.FileName, data.FrameIndex, Nothing))
                    End If
                    If i > 0 Then
                        encoder.Append = True
                    End If
                    fs.Seek(0, SeekOrigin.Begin)
                    encoder.Save(fs, image, Nothing)
                    If i <> _currentIndex Then
                        image.Dispose()
                    End If
                    i += 1
                Loop
            Finally
                Dim disp As IDisposable = fs
                disp.Dispose()
            End Try
            'INSTANT VB NOTE: End of the original C# 'using' block
        End Sub

        Private Sub SaveAnimatedGif()
            Dim col As GifFrameCollection = New GifFrameCollection
            Dim frame As GifFrame = Nothing
            Dim frm As MultiSave = New MultiSave
            Dim gif As GifEncoder = New GifEncoder

            ' Make the encoder the first item.
            frm.listImages.Items.Add("GifEncoder")
            frm.images.Add(gif)

            ' Add each image as a frame.
            Dim i As Integer = 0
            'ORIGINAL LINE: for (int i = 0; i < _images.Count; i += 1)
            'INSTANT VB NOTE: This 'for' loop was translated to a VB 'Do While' loop:
            Do While i < _images.Count
                Dim data As ImageFileLoadData = CType(_images(i), ImageFileLoadData)
                Dim image As AtalaImage
                If i = _currentIndex Then
                    image = (Viewer.Image)
                Else
                    image = (New AtalaImage(data.FileName, data.FrameIndex, Nothing))
                End If
                frm.listImages.Items.Add(image.ToString())
                frame = New GifFrame(image)
                col.Add(frame)
                frm.images.Add(frame)
                i += 1
            Loop

            ' Select the first image.
            frm.listImages.SelectedIndex = 0

            If frm.ShowDialog(Me) = DialogResult.OK Then

                Me.Cursor = Cursors.WaitCursor

                Dim fs As FileStream = New FileStream(Me.currentFile, FileMode.Create, FileAccess.Write)
                gif.Save(fs, col, Nothing)
                fs.Close()

                Me.Cursor = Cursors.Default
            End If

            ' Dispose the temporary images.
            i = 1
            'ORIGINAL LINE: for (int i = 1; i <= _images.Count; i += 1)
            'INSTANT VB NOTE: This 'for' loop was translated to a VB 'Do While' loop:
            Do While i <= _images.Count
                If i - 1 <> _currentIndex Then
                    CType(frm.images(i), GifFrame).Image.Dispose()
                End If
                i += 1
            Loop

            frm.Dispose()

        End Sub

        Private Sub SavePdf()
            Dim col As PdfImageCollection = New PdfImageCollection
            Dim page As PdfImage = Nothing
            Dim frm As MultiSave = New MultiSave
            Dim pdf As PdfEncoder = New PdfEncoder

            ' Make the encoder the first item.
            frm.listImages.Items.Add("PdfEncoder")
            frm.images.Add(pdf)

            ' Add each image.
            Dim i As Integer = 0
            'ORIGINAL LINE: for (int i = 0; i < _images.Count; i += 1)
            'INSTANT VB NOTE: This 'for' loop was translated to a VB 'Do While' loop:
            Do While i < _images.Count
                Dim data As ImageFileLoadData = CType(_images(i), ImageFileLoadData)
                Dim image As AtalaImage
                If i = _currentIndex Then
                    image = (Viewer.Image)
                Else
                    image = (New AtalaImage(data.FileName, data.FrameIndex, Nothing))
                End If
                frm.listImages.Items.Add(image.ToString())
                page = New PdfImage(image, PdfCompressionType.Auto)
                col.Add(page)
                frm.images.Add(page)
                i += 1
            Loop

            ' Select the first image.
            frm.listImages.SelectedIndex = 0

            If frm.ShowDialog(Me) = DialogResult.OK Then
                Me.Cursor = Cursors.WaitCursor

                Dim fs As FileStream = New FileStream(Me.currentFile, FileMode.Create, FileAccess.Write)
                pdf.Save(fs, col, Nothing)
                fs.Close()

                Me.Cursor = Cursors.Default
            End If

            ' Dispose the temporary images.
            i = 1
            'ORIGINAL LINE: for (int i = 1; i <= _images.Count; i += 1)
            'INSTANT VB NOTE: This 'for' loop was translated to a VB 'Do While' loop:
            Do While i <= _images.Count
                If i - 1 <> _currentIndex Then
                    CType(frm.images(i), PdfImage).Image.Dispose()
                End If
                i += 1
            Loop

            frm.Dispose()
        End Sub

        Private Sub ReadMetadata()
            ' A Photo Pro or Document license is required for metadata.
            If AtalaImage.Edition = LicenseEdition.Photo OrElse AtalaImage.Edition = LicenseEdition.Standard Then
                Return
            End If

            If Me.currentFile.Length = 0 OrElse Me.currentFile.StartsWith("http") Then
                Return
            End If

            Dim fs As FileStream = New FileStream(Me.currentFile, FileMode.Open, FileAccess.Read)
            Dim exif As ExifParser = New ExifParser
            Me.exifItems = exif.ParseFromImage(fs, 0)
            Me.menuImageExif.Enabled = (Not Me.exifItems Is Nothing AndAlso Me.exifItems.Count > 0)

            fs.Seek(0, SeekOrigin.Begin)
            Dim iptc As IptcParser = New IptcParser
            Me.iptcItems = iptc.ParseFromImage(fs, 0)
            Me.menuImageIptc.Enabled = (Not Me.iptcItems Is Nothing AndAlso Me.iptcItems.Count > 0)

            fs.Seek(0, SeekOrigin.Begin)
            Me.jpegMarkers = New JpegMarkerCollection(fs)

            fs.Seek(0, SeekOrigin.Begin)
            Dim com As ComTextParser = New ComTextParser
            Me.comItems = com.ParseFromImage(fs)

            fs.Close()
        End Sub

        Private Sub LoadDirect(ByVal image As AtalaImage)
            Viewer.Images.Clear()
            Viewer.Images.Add(image)

            _images.Clear()
            _currentIndex = 0

            Dim tmp As String = Path.GetTempFileName()
            image.Save(tmp, New PngEncoder, Nothing)
            _tempFiles.Add(tmp)
            _images.Add(New ImageFileLoadData(tmp, 0))

            UpdateUndoRedoInfo()
            EnableMenuItems()
        End Sub

#End Region

#Region "Rubberband Drawing Event Handlers"
        Private Sub lineDraw_Changed(ByVal sender As Object, ByVal e As Atalasoft.Imaging.WinControls.RubberbandEventArgs) Handles lineDraw.Changed
            Dim myCanvas As Canvas = New Canvas(Me.Viewer.Image)
            myCanvas.SmoothingLevel = Me.canvasSmoothing
            myCanvas.DrawLine(New Point(CInt(e.StartPoint.X), CInt(e.StartPoint.Y)), New Point(CInt(e.EndPoint.X), CInt(e.EndPoint.Y)), lineDraw.Pen)
            Viewer.Refresh()

            If Me.drawMode = DrawMenuMode.Lines Then
                lineDraw.Start(e.EndPoint, True)
            ElseIf Me.drawMode = DrawMenuMode.Polygon Then
                If Me.polygonPoints Is Nothing Then
                    Me.polygonPoints = New Point(1) {}
                    Me.polygonPoints(0) = New Point(CInt(e.StartPoint.X), CInt(e.StartPoint.Y))
                    Me.polygonPoints(1) = New Point(CInt(e.EndPoint.X), CInt(e.EndPoint.Y))
                Else
                    Dim tmp As Point() = Me.polygonPoints
                    Me.polygonPoints = New Point(tmp.Length) {}
                    tmp.CopyTo(Me.polygonPoints, 0)
                    Me.polygonPoints(tmp.Length) = New Point(CInt(e.EndPoint.X), CInt(e.EndPoint.Y))
                End If

                lineDraw.Start(e.EndPoint, True)
            End If

            UpdateUndoRedoInfo()

        End Sub
        Private Sub rectangleDraw_Changed(ByVal sender As Object, ByVal e As Atalasoft.Imaging.WinControls.RubberbandEventArgs) Handles rectangleDraw.Changed
            Dim myCanvas As Canvas = New Canvas(Me.Viewer.Image)
            myCanvas.SmoothingLevel = Me.canvasSmoothing
            myCanvas.DrawRectangle(e.GetBounds(), Me.rectangleDraw.Pen, Me.rectangleDraw.Fill, Me.rectangleDraw.CornerRadius)
            Viewer.Refresh()
            UpdateUndoRedoInfo()
        End Sub

        Private Sub ellipseDraw_Changed(ByVal sender As Object, ByVal e As Atalasoft.Imaging.WinControls.RubberbandEventArgs) Handles ellipseDraw.Changed
            Dim myCanvas As Canvas = New Canvas(Me.Viewer.Image)
            myCanvas.SmoothingLevel = Me.canvasSmoothing
            myCanvas.DrawEllipse(e.GetBounds(), Me.ellipseDraw.Pen, Me.ellipseDraw.Fill)
            Viewer.Refresh()
            Me.ellipseDraw.Visible = False
            UpdateUndoRedoInfo()
        End Sub
#End Region

#Region "File Menu"
        'create a new image
        Private Sub menuFileNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileNew.Click
            ' Start a new image.
            Dim newImage As NewImageParameter = New NewImageParameter
            Dim frm As Form = New Parameters("New Image", newImage)

            If frm.ShowDialog(Me) = DialogResult.OK Then
                ' Check for a valid entry.
                If newImage.Width < 2 OrElse newImage.Height < 2 Then
                    MessageBox.Show("You have provided an invalid width or height.")
                    frm.Dispose()
                    Return
                End If

                ' Pass the new image.
                Dim image As AtalaImage = New AtalaImage(newImage.Width, newImage.Height, newImage.ImageFormat, newImage.BackColor)
                Viewer.Undos.Add("New Image", False)
                LoadDirect(image)
            End If
        End Sub

        'open an image from a file
        Private Sub menuFileOpen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileOpen.Click
            OpenAndLoadImage()
        End Sub

        'open an image from a URL
        Private Sub menuFileOpenFromURL_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileOpenFromURL.Click
            Dim url As UrlParameter = New UrlParameter
            Dim frm As Form = New Parameters("Open from URL", url)
            If frm.ShowDialog(Me) = DialogResult.OK Then
                Viewer.Open(url.Url)
            End If
        End Sub

        Private Sub menuFileDecoders_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileDecoders.Click, menuFileJPEG.Click, menuFilePDF.Click, menuFilePNG.Click, menuFileTLA.Click, menuFileWMF.Click
            Dim item As MenuItem = CType(sender, MenuItem)
            Dim decoder As ImageDecoder = Nothing
            Select Case item.Text
                Case "JPEG"
                    decoder = RegisteredDecoders.GetDecoderFromType(GetType(JpegDecoder))
                Case "TLA"
                    decoder = RegisteredDecoders.GetDecoderFromType(GetType(TlaDecoder))
                    Exit Select
#If PDFRasterizer Then
                Case "PDF"
                    decoder = RegisteredDecoders.GetDecoderFromType(GetType(PdfDecoder))
                    Exit Select
#End If
                Case "PNG"
                    decoder = RegisteredDecoders.GetDecoderFromType(GetType(PngDecoder))
                Case "WMF"
                    decoder = RegisteredDecoders.GetDecoderFromType(GetType(WmfDecoder))
            End Select

            'show properties.  Changed Settings will be saved to the KnownDecoders class
            Dim frm As Parameters = New Parameters(decoder.ToString() & " Settings", decoder)
            frm.ShowDialog(Me)

        End Sub

        'generate a new image containing noise
        Private Sub menuFileNoise_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileNoise.Click
            Dim noise As NoiseGenerator = New NoiseGenerator(New Size(400, 300), 0.5)
            Dim noiseForm As Parameters = New Parameters("Noise Generator", noise)
            Dim image As AtalaImage = Nothing
            If noiseForm.ShowDialog() = DialogResult.OK Then
                Select Case noise.Mode
                    Case NoiseGeneratorMode.Hugo
                        image = noise.GenerateImage(2, 2.5)
                    Case NoiseGeneratorMode.DimensionalSlice
                        image = noise.GenerateImage(10)
                    Case Else
                        image = noise.GenerateImage()
                End Select
            End If
            noiseForm.Dispose()
            LoadDirect(image)
        End Sub

        Private Sub menuFileSaveAs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileSaveAs.Click
            Me.SaveCurrentImage(Nothing)
        End Sub

        Private Sub menuFileSaveFTP_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileSaveFTP.Click
            Dim ftp As UrlParameter = New UrlParameter
            Dim frm As Form = New Parameters("Save to FTP", ftp)
            If frm.ShowDialog(Me) = DialogResult.OK Then
                Me.SaveCurrentImage(ftp)
            End If
            frm.Dispose()

        End Sub

        'shows the page setup dialog to set the page settings prior to printing
        Private Sub menuFilePageSetup_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFilePageSetup.Click
            Me.pageSetupDialog1.Document = Me.imagePrintDocument1
            Me.pageSetupDialog1.ShowDialog(Me)
        End Sub

        'show the print dialog, and uses the ImagePrintDocument component to print images
        Private Sub menuFilePrintImage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFilePrintImage.Click
            Me.printDialog1.Document = Me.imagePrintDocument1
            If Me.printDialog1.ShowDialog(Me) = DialogResult.OK Then
                Me.Viewer.Image.Resolution = New Dpi(96, 96, Atalasoft.Imaging.ResolutionUnit.DotsPerInch)
                Me.imagePrintDocument1.Image = Me.Viewer.Image
                Dim printParams As Parameters = New Parameters("Print Options", Me.imagePrintDocument1)
                If printParams.ShowDialog() = DialogResult.OK Then
                    Me.imagePrintDocument1.Print()
                End If
                printParams.Dispose()
            End If
        End Sub

        'exit the application
        Private Sub menuFileExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFileExit.Click
            Application.Exit()
        End Sub
#End Region

#Region "Edit Menu"
        'undo the last image change
        Private Sub menuEditUndo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEditUndo.Click
            Viewer.Undos.Undo()
            UpdateUndoRedoInfo()
        End Sub

        'redo the previous undo
        Private Sub menuEditRedo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEditRedo.Click
            Viewer.Undos.Redo()
            UpdateUndoRedoInfo()
        End Sub

        Private Sub CopyImageToClipboard(ByVal cutArea As Boolean)
            If _images.Count > 0 Then
                ' Copy to the clipboard and erase that area.
                If Not Viewer.Selection Is Nothing AndAlso Viewer.Selection.Visible AndAlso Viewer.Selection.Bounds.Width > 0 AndAlso Viewer.Selection.Bounds.Height > 0 Then
                    ' Crop to the selection.
                    Dim copy As CropCommand = New CropCommand(Viewer.Selection.Bounds)
                    Dim copyImage As AtalaImage = copy.Apply(Viewer.Image).Image
                    copyImage.CopyToClipboard(Me.Handle)

                    ' A copy of the image is sent to the clipboard,
                    ' so go ahead and distroy this image.
                    copyImage.Dispose()

                    If cutArea Then
                        ' Erase it by simply drawing a solid region.
                        Dim myCanvas As Canvas = New Canvas(Viewer.Image)
                        myCanvas.DrawRegion(Viewer.Selection.GetRegion(), New SolidFill(Color.White))
                        Viewer.Refresh()
                    End If
                Else
                    Viewer.Image.CopyToClipboard(Me.Handle)
                End If
            End If
        End Sub

        'cut the selected porting of the image
        Private Sub menuEditCut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEditCut.Click
            CopyImageToClipboard(True)
        End Sub

        Private Sub menuEditCopy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEditCopy.Click
            CopyImageToClipboard(False)
        End Sub

        Private Sub menuEditPaste_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEditPaste.Click
            Dim pasteImage As AtalaImage = AtalaImage.ImageFromClipboard(Me.Handle)
            If pasteImage Is Nothing Then
                Return
            End If

            If _images.Count = 0 Then
                Viewer.Images.Add(pasteImage)
                Viewer.Update()
                Return
            End If

            ' You cannot overlay onto a 4-bit image.
            If Viewer.Image.PixelFormat = PixelFormat.Pixel4bppIndexed Then
                Dim cp As ChangePixelFormatCommand = New ChangePixelFormatCommand(PixelFormat.Pixel8bppIndexed)
                Viewer.ApplyCommand(cp)
            End If

            ' You also cannot overlay an 16-bit image onto an 8-bit indexed.

            ' Ask where the image should be placed.
            Dim pt As PointParameter = New PointParameter
            If Not Viewer.Selection Is Nothing AndAlso Viewer.Selection.Visible Then
                pt.Position = Viewer.Selection.Bounds.Location
            End If

            Dim frm As Form = New Parameters("Paste Image", pt)
            If frm.ShowDialog() = DialogResult.OK Then
                ' Turn off asynchrouous so the paste image can be disposed.
                Dim async As Boolean = Viewer.Asynchronous
                Viewer.Asynchronous = False
                Dim ovr As OverlayCommand = New OverlayCommand(pasteImage, pt.Position, CDbl(1))
                Viewer.ApplyCommand(ovr, "Paste")
                Viewer.Asynchronous = async
            End If
            frm.Dispose()
            pasteImage.Dispose()
        End Sub

        Private Sub menuEditOptions_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEditOptions.Click
            Dim opts As ProgramOptions = New ProgramOptions
            opts.Asynchronous = Viewer.Asynchronous
            opts.UndoLevels = Viewer.Undos.Levels
            opts.AntialiasDisplay = Viewer.AntialiasDisplay
            opts.AutoZoom = Viewer.AutoZoom
            opts.ScrollBarStyle = Viewer.ScrollBarStyle

            Dim frm As Form = New Parameters("Program Options", opts)
            If frm.ShowDialog() = DialogResult.OK Then
                ' Set the modified options.
                Viewer.Asynchronous = opts.Asynchronous
                Viewer.Undos.Levels = opts.UndoLevels
                Viewer.AntialiasDisplay = opts.AntialiasDisplay
                Viewer.AutoZoom = opts.AutoZoom
                Viewer.ScrollBarStyle = opts.ScrollBarStyle
            End If
            frm.Dispose()
        End Sub
#End Region

#Region "Image Menu"
        'show image information
        Private Sub menuImageInformation_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuImageInformation.Click
            Dim frm As Form = New Parameters("Image Information", Viewer.Image)
            frm.ShowDialog()
            frm.Dispose()
        End Sub

        Private Sub menuImageChangePixelFormat_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuImageChangePixelFormat.Click
            ShowCommand("Change Pixel Format", New ChangePixelFormatCommand(Viewer.Image.PixelFormat))
        End Sub

        Private Sub menuImageShowHistogram_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuImageShowHistogram.Click
            Me.histogram = New Histogram
            Try
                histogram.SetHistogram(Viewer.Image)
                histogram.Show()
            Catch err As System.Exception
                MessageBox.Show(Me, err.Message)
                Me.histogram = Nothing
            End Try
        End Sub

        Private Sub menuImageExif_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuImageExif.Click
            ' TODO: Setup a custom class so the values can be used.
            Dim eTags As ExifTag() = New ExifTag(Me.exifItems.Count - 1) {}
            Me.exifItems.CopyTo(eTags, 0)
            Dim frm As Form = New Parameters("EXIF Data", eTags)
            frm.ShowDialog()
            frm.Dispose()
        End Sub

        Private Sub menuImageIptc_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuImageIptc.Click
            Dim iTags As IptcTag() = New IptcTag(Me.iptcItems.Count - 1) {}
            Me.iptcItems.CopyTo(iTags, 0)
            Dim frm As Form = New Parameters("IPTC Data", iTags)
            frm.ShowDialog()
            frm.Dispose()
        End Sub

        'set the viewer zoom
        Private Sub menuImageZoom_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuImageZoom132.Click, menuImageZoom116.Click, menuImageZoom18.Click, menuImageZoom25.Click, menuImageZoom50.Click, menuImageZoom100.Click, menuImageZoom200.Click, menuImageZoom500.Click, menuImageZoom1000.Click, menuImageZoom31.Click
            Dim item As MenuItem = CType(sender, MenuItem)
            Select Case item.Index
                Case 0
                    Viewer.Zoom = CDbl(1) / 32
                Case 1
                    Viewer.Zoom = CDbl(1) / 16
                Case 2
                    Viewer.Zoom = CDbl(1) / 8
                Case 3
                    Viewer.Zoom = 0.25
                Case 4
                    Viewer.Zoom = 0.5
                Case 5
                    Viewer.Zoom = 1.0
                Case 6
                    Viewer.Zoom = 2.0
                Case 7
                    Viewer.Zoom = 5.0
                Case 8
                    Viewer.Zoom = 10.0
                Case 9
                    Viewer.Zoom = CDbl(1) / 32
            End Select

            Me.Viewer.Magnifier.Zoom = Viewer.Zoom * 4
        End Sub
#End Region

#Region "Channels Commands"
        Private Sub menuChannelsCombine_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsCombine.Click
            If MessageBox.Show("This command will combine multiple 8-bit images into a single image." & Constants.vbLf & "Each image represents one channel of the image." & Constants.vbLf + Constants.vbLf & "Do you want to continue?", "Combine", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                Dim combineFile As OpenFileDialog = New OpenFileDialog
                combineFile.Filter = HelperMethods.CreateDialogFilter(True)

                Dim images As AtalaImage() = New AtalaImage(3) {}

                ' channel 1
                combineFile.Title = "Channel 1"
                If combineFile.ShowDialog() <> DialogResult.OK Then
                    Return
                End If
                images(0) = New AtalaImage(combineFile.FileName)

                ' channel 2
                combineFile.Title = "Channel 2"
                If combineFile.ShowDialog() <> DialogResult.OK Then
                    images(0).Dispose()
                    Return
                End If
                images(1) = New AtalaImage(combineFile.FileName)

                ' channel 3 (optional)
                combineFile.Title = "Channel 3 (optional)"
                If combineFile.ShowDialog() = DialogResult.OK Then
                    images(2) = New AtalaImage(combineFile.FileName)
                End If

                ' channel 4 (optional)
                combineFile.Title = "Channel 4 (optional)"
                If combineFile.ShowDialog() = DialogResult.OK Then
                    images(3) = New AtalaImage(combineFile.FileName)
                End If

                Dim combine As AtalaImage = Nothing

                If Not images(3) Is Nothing Then
                    combine = AtalaImage.CombineChannels(PixelFormat.Pixel32bppBgra, images(0), images(1), images(2), images(3))
                ElseIf Not images(2) Is Nothing Then
                    combine = AtalaImage.CombineChannels(PixelFormat.Pixel24bppBgr, images(0), images(1), images(2))
                Else
                    combine = AtalaImage.CombineChannels(PixelFormat.Pixel16bppGrayscaleAlpha, images(0), images(1))
                End If
                For Each image As AtalaImage In images
                    If Not image Is Nothing Then
                        image.Dispose()
                    End If
                Next image

                Viewer.Undos.Add("Combine Image Channels", False)
                Viewer.Image = combine
                Me.UpdateUndoRedoInfo()
            End If

        End Sub

        Private Sub menuChannelsReplace_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsReplace.Click
            ' Make sure there are at least two channels.
            Dim channels As Integer = 0
            Select Case Me.Viewer.Image.PixelFormat
                Case PixelFormat.Pixel16bppGrayscale, PixelFormat.Pixel1bppIndexed, PixelFormat.Pixel4bppIndexed, PixelFormat.Pixel8bppGrayscale, PixelFormat.Pixel8bppIndexed
                    MessageBox.Show("Replace channels only works on images with at least two color channels.", "Invalid Pixel Format", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                Case PixelFormat.Pixel16bppGrayscaleAlpha
                    channels = 2
                Case PixelFormat.Pixel24bppBgr, PixelFormat.Pixel48bppBgr
                    channels = 3
                Case Else
                    channels = 4
            End Select

            If MessageBox.Show("This command will replace any or all channels in an image with an 8-bit image." & Constants.vbLf & "To skip a channel, simply click the 'Cancel' button in the file dialog." & Constants.vbLf + Constants.vbLf & "Do you want to continue?", "Replace", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                Dim replaceFile As OpenFileDialog = New OpenFileDialog
                replaceFile.Filter = HelperMethods.CreateDialogFilter(True)

                Dim images As AtalaImage() = New AtalaImage(3) {}

                ' Channel 1
                replaceFile.Title = "Channel 1"
                If replaceFile.ShowDialog() = DialogResult.OK Then
                    images(0) = New AtalaImage(replaceFile.FileName)
                    If (images(0).PixelFormat <> PixelFormat.Pixel8bppGrayscale) Then
                        MessageBox.Show(Me, "You can only replace a channel with an 8bpp grayscale image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                End If

                ' Channel 2
                replaceFile.Title = "Channel 2"
                If replaceFile.ShowDialog() = DialogResult.OK Then
                    images(1) = New AtalaImage(replaceFile.FileName)
                    If (images(1).PixelFormat <> PixelFormat.Pixel8bppGrayscale) Then
                        MessageBox.Show(Me, "You can only replace a channel with an 8bpp grayscale image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                End If

                ' Channel 3
                If channels > 2 Then
                    replaceFile.Title = "Channel 3"
                    If replaceFile.ShowDialog() = DialogResult.OK Then
                        images(2) = New AtalaImage(replaceFile.FileName)
                        If (images(2).PixelFormat <> PixelFormat.Pixel8bppGrayscale) Then
                            MessageBox.Show(Me, "You can only replace a channel with an 8bpp grayscale image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End If
                    End If
                End If

                ' Channel 4
                If channels > 3 Then
                    replaceFile.Title = "Channel 4"
                    If replaceFile.ShowDialog() = DialogResult.OK Then
                        images(3) = New AtalaImage(replaceFile.FileName)
                        If (images(3).PixelFormat <> PixelFormat.Pixel8bppGrayscale) Then
                            MessageBox.Show(Me, "You can only replace a channel with an 8bpp grayscale image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End If
                    End If
                End If

                Dim replaceChannel As ReplaceChannelCommand = New ReplaceChannelCommand(images(0), images(1), images(2), images(3))
                Viewer.ApplyCommand(replaceChannel, "Replace Channels")
                UpdateUndoRedoInfo()
            End If
        End Sub

        ' Split the image into 8-bit channel images.
        Private Sub menuChannelsSplit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsSplit.Click
            If Me.Viewer.Image.ColorDepth < 16 Then
                Return
            End If

            Dim images As AtalaImage() = Me.Viewer.Image.SplitChannels(ChannelFlags.AllChannels)
            If images.Length = 0 Then
                Return
            End If

            Me.Viewer.Images.Clear()
            _images.Clear()
            Me.Viewer.Images.Add(images(0))

            Dim i As Integer = 0
            'ORIGINAL LINE: for (int i = 0; i < images.Length; i += 1)
            'INSTANT VB NOTE: This 'for' loop was translated to a VB 'Do While' loop:
            Do While i < images.Length
                Dim tmp As String = Path.GetTempFileName()
                images(i).Save(tmp, New PngEncoder, Nothing)
                If i > 0 Then
                    images(i).Dispose()
                End If
                _tempFiles.Add(tmp)
                _images.Add(New ImageFileLoadData(tmp, 0))
                i += 1
            Loop

            UpdateUndoRedoInfo()
        End Sub

        Private Sub menuChannelsAdjust_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsAdjust.Click
            ShowCommand("Adjust Channels", New AdjustChannelCommand(False, 0, 0, 0))
        End Sub

        Private Sub menuChannelsAdjustHsl_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsAdjustHsl.Click
            ShowCommand("Adjust HSL", New AdjustHslCommand(False, 0, 0, 0))
        End Sub

        Private Sub menuChannelsApplyLut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsApplyLut.Click
            ' Use ApplyLut to create a negative image.
            Dim ch1 As Byte() = New Byte(255) {}
            Dim ch2 As Byte() = New Byte(255) {}
            Dim ch3 As Byte() = New Byte(255) {}
            Dim ch4 As Byte() = New Byte(255) {}

            For i As Integer = 0 To 255
                ch1(i) = CByte(255 - i)
                ch2(i) = CByte(255 - i)
                ch3(i) = CByte(255 - i)
                ch4(i) = CByte(255 - i)
            Next i

            ShowCommand("Apply LUT (invert)", New ApplyLutCommand(ch1, ch2, ch3, ch4))
        End Sub

        Private Sub menuChannelsInvert_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsInvert.Click
            ShowCommand("Invert", New InvertCommand)
        End Sub

        Private Sub menuChannelsShift_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsShift.Click
            ShowCommand("Shift Channels", New ShiftChannelsCommand(20, 20, ChannelFlags.Channel1, 255))
        End Sub

        Private Sub menuChannelsSwap_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsSwap.Click
            ShowCommand("Swap Channels", New SwapChannelsCommand(1, 2, 3, 4))
        End Sub

        Private Sub menuChannelsFlattenAlpha_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsFlattenAlpha.Click
            ShowCommand("Flatten Alpha", New FlattenAlphaCommand(Color.White))
        End Sub

        Private Sub menuChannelsAphaColor_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsAphaColor.Click
            ShowCommand("Set Alpha By Color", New SetAlphaColorCommand(Color.White))
        End Sub

        Private Sub menuChannelsAlphaMask_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsAlphaMask.Click
            Dim alphaFile As OpenFileDialog = New OpenFileDialog
            alphaFile.Filter = HelperMethods.CreateDialogFilter(True)
            Dim mask As AtalaImage = Nothing
            If alphaFile.ShowDialog() = DialogResult.OK Then
                mask = New AtalaImage(alphaFile.FileName)
            End If
            alphaFile.Dispose()
            If mask Is Nothing Then
                Return
            End If

            ShowCommand("Set Alpha From Mask", New SetAlphaFromMaskCommand(mask, True, AlphaMergeType.UseMostTransparent))
        End Sub

        Private Sub menuChannelsAlphaValue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuChannelsAlphaValue.Click
            ShowCommand("Set Alpha Value", New SetAlphaValueCommand(128, AlphaMergeType.UseMostTransparent))
        End Sub
#End Region

#Region "Effects Commands"


        Private Sub menuEffectsSaturation_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsSaturation.Click
            ShowCommand("Saturation", New SaturationCommand)
        End Sub

        Private Sub menuEffectsAdjustTint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsAdjustTint.Click
            ShowCommand("Adjust Tint", New AdjustTintCommand(60))
        End Sub

        Private Sub menuEffectsBevelEdge_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsBevelEdge.Click
            ShowCommand("Bevel Edge", New BevelEdgeCommand(12, 150, 150, 60, 60, 100))
        End Sub

        Private Sub menuEffectsCrackle_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsCrackle.Click
            ShowCommand("Crackle", New CrackleCommand(CrackleMode.Erosion, 20))
        End Sub

        Private Sub menuEffectsDeInterlace_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsDeInterlace.Click
            ShowCommand("De-Interlace", New DeInterlaceCommand)
        End Sub

        Private Sub menuEffectsDropShadow_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsDropShadow.Click
            ShowCommand("Drop Shadow", New DropShadowCommand(10))
        End Sub

        Private Sub menuEffectsFingerPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsFingerPrint.Click
            ShowCommand("Fingerprint", New FingerprintCommand(50, 5, True))
        End Sub

        Private Sub menuEffectsFloodFill_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsFloodFill.Click
            ShowCommand("Flood Fill", New FloodFillCommand(New Point(0, 0), Color.Black, 0, ColorMatchMode.Surface))
        End Sub

        Private Sub menuEffectsGamma_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsGamma.Click
            ShowCommand("Gamma", New GammaCommand(1))
        End Sub

        Private Sub menuEffectsGauzy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsGauzy.Click
            ShowCommand("Gauzy", New GauzyCommand(50, 20, 60, GauzyMode.Max))
        End Sub

        Private Sub menuEffectsHalftone_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsHalftone.Click
            ShowCommand("Halftone", New HalftoneCommand(2, False))
        End Sub

        Private Sub menuEffectsMosaic_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsMosaic.Click
            ShowCommand("Mosaic", New MosaicCommand(20))
        End Sub

        Private Sub menuEffectsOilPaint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsOilPaint.Click
            ShowCommand("Oil Paint", New OilPaintCommand)
        End Sub

        Private Sub menuEffectsPosterize_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsPosterize.Click
            ShowCommand("Posterize", New PosterizeCommand)
        End Sub

        Private Sub menuReduceColors_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuReduceColors.Click
            ShowCommand("Reduce Colors", New ReduceColorsCommand(256))
        End Sub

        Private Sub menuEffectsReplaceColor_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsReplaceColor.Click
            ShowCommand("Replace Color", New ReplaceColorCommand(Color.White, Color.Gray, 0))
        End Sub

        Private Sub menuEffectsRoundedBevel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsRoundedBevel.Click
            ShowCommand("Rounded Bevel", New RoundedBevelCommand)
        End Sub

        Private Sub menuEffectsSolarize_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsSolarize.Click
            ShowCommand("Solarize", New SolarizeCommand)
        End Sub

        Private Sub menuEffectsStipple_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsStipple.Click
            ShowCommand("Stipple", New StippleCommand(40, StippleFilterType.GeometricMean, StippleMode.FilterFirst))
        End Sub

        Private Sub menuEffectsTintGrayscale_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsTintGrayscale.Click
            ShowCommand("Tint Grayscale", New TintGrayscaleCommand(Color.Azure))
        End Sub

        Private Sub menuEffectsWatercolorTint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsWatercolorTint.Click
            ShowCommand("Watercolor Tint", New WatercolorTintCommand)
        End Sub

        Private Sub menuEffectsBrightnessHistogramEqualize_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsBrightnessHistogramEqualize.Click
            ShowCommand("Brightness Histogram Equalize", New BrightnessHistogramEqualizeCommand(64, 200))
        End Sub

        Private Sub menuEffectsBrightnessHistogramStretch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsBrightnessHistogramStretch.Click
            ShowCommand("Brightness Histogram Stretch", New BrightnessHistogramStretchCommand(10, 10))
        End Sub

        Private Sub menuHistogramEqualize_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuHistogramEqualize.Click
            ShowCommand("Histogram Equalize", New HistogramEqualizeCommand(10, 200))
        End Sub

        Private Sub menuEffectsHistogramStretch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsHistogramStretch.Click
            ShowCommand("Histogram Stretch", New HistogramStretchCommand(10, 10))
        End Sub

        Private Sub menuEffectsRedEyeRemoval_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsRedEyeRemoval.Click
            ShowCommand("Red Eye Removal", New RedEyeRemovalCommand)
        End Sub

        Private Sub menuEffectsLevels_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsLevels.Click
            ShowCommand("Levels", New LevelsCommand)
        End Sub

        Private Sub menuEffectsAutoLevels_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsAutoLevels.Click
            ShowCommand("Auto Levels", New AutoLevelsCommand)
        End Sub

        Private Sub menuEffectsAutoContrast_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsAutoContrast.Click
            ShowCommand("Auto Contrast", New AutoContrastCommand)
        End Sub

        Private Sub menuEffectsAutoColor_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsAutoColor.Click
            ShowCommand("Auto Color", New AutoColorCommand)
        End Sub

        Private Sub menuEffectsAutoWhiteBalance_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsAutoWhiteBalance.Click
            ShowCommand("Auto White Balance", New AutoWhiteBalanceCommand)
        End Sub
#End Region

#Region "Filters Commands"
        Private Sub menuFiltersBrightnessContrast_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersBrightnessContrast.Click
            ShowCommand("Brightness / Contrast", New BrightnessContrastCommand(0, 0))
        End Sub

        Private Sub menuFiltersSaturation_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersSaturation.Click
            ShowCommand("Saturation", New SaturationCommand(1))
        End Sub

        Private Sub menuFiltersBlur_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersBlur.Click
            ShowCommand("Blur", New BlurCommand(60, 3))
        End Sub

        Private Sub menuFiltersGaussianBlur_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersGaussianBlur.Click
            ShowCommand("Gaussian Blur", New BlurGaussianCommand(2.4))
        End Sub

        Private Sub menuAdaptiveUnsharpMask_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuAdaptiveUnsharpMask.Click
            ShowCommand("Adaptive Unsharp Mask", New AdaptiveUnsharpMaskCommand(0, 2.5, AdaptiveUnsharpQuality.Middle))
        End Sub

        Private Sub menuEffectsUnsharpMask_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuEffectsUnsharpMask.Click
            ShowCommand("Unsharp Mask", New UnsharpMaskCommand(0, 1, 2.4))
        End Sub

        Private Sub menuFiltersSharpen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersSharpen.Click
            ShowCommand("Sharpen", New SharpenCommand(80, 3))
        End Sub

        Private Sub menuFiltersAddNoise_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersAddNoise.Click
            ShowCommand("Add Noise", New AddNoiseCommand(AddNoiseFilterType.Negative, 600, False))
        End Sub

        Private Sub menuFiltersDespeckle_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersDespeckle.Click
            ShowCommand("Despeckle", New DespeckleCommand)
        End Sub

        Private Sub menuFiltersEmboss_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersEmboss.Click
            ShowCommand("Emboss", New EmbossCommand(135, 10, 135, False))
        End Sub

        Private Sub menuFiltersIntensify_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersIntensify.Click
            ShowCommand("Intensify", New IntensifyCommand(50))
        End Sub

        Private Sub menuFiltersHighPass_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersHighPass.Click
            ShowCommand("High Pass Filters", New HighPassCommand(4, 50))
        End Sub

        Private Sub menuFiltersMaximum_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersMaximum.Click
            ShowCommand("Maximum Filter", New MaximumCommand(3))
        End Sub

        Private Sub menuFiltersMean_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersMean.Click
            ShowCommand("Mean Filters", New MeanCommand(MeanFilterType.Arithmetic, 3, 2))
        End Sub

        Private Sub menuFiltersMedian_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersMedian.Click
            ShowCommand("Median Filter", New MedianCommand)
        End Sub

        Private Sub menuFiltersMidpoint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersMidpoint.Click
            ShowCommand("Midpoint Filter", New MidpointCommand(3))
        End Sub

        Private Sub menuFiltersMinimum_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersMinimum.Click
            ShowCommand("Minimum Filter", New MinimumCommand(3))
        End Sub

        Private Sub menuFiltersMorphological_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersMorphological.Click, menuMorphoDilation.Click, menuMorphoErosion.Click, menuMorphoOpen.Click, menuMorphoClose.Click, menuMorphoTophat.Click, menuMorphoGradient.Click
            Dim item As MenuItem = CType(sender, MenuItem)
            Dim command As MorphoGrayCommand = New MorphoGrayCommand(CType(item.Index, MorphoGrayMode))
            command.RegionOfInterest = GetRegionOfInterest()
            Viewer.ApplyCommand(command, "Morphological " & item.Text)
            UpdateUndoRedoInfo()
        End Sub

        Private Sub menuFiltersThreshold_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersThreshold.Click
            ShowCommand("Threshold Filter", New ThresholdCommand(10, 200))
        End Sub

        Private Sub menuFiltersConvolutionFilter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersConvolutionFilter.Click
            Dim matrix As DoubleMatrix = New DoubleMatrix(3, 3)
            matrix.SetRow(0, 1, 1, 1)
            matrix.SetRow(1, 1, -8, 1)
            matrix.SetRow(2, 1, 1, 1)

            ShowCommand("Convolution Filter", New ConvolutionFilterCommand(matrix, 0.8))
        End Sub

        Private Sub menuFiltersConvolutionMatrix_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersConvolutionMatrix.Click
            Dim matrix As DoubleMatrix = New DoubleMatrix(3, 3)
            matrix.SetRow(0, 1, 1, 1)
            matrix.SetRow(1, 1, -8, 1)
            matrix.SetRow(2, 1, 1, 1)

            ShowCommand("Convolution Matrix", New ConvolutionMatrixCommand(matrix, True, 0.8))
        End Sub

        Private Sub menuFiltersEdge_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersEdge.Click
            ShowCommand("Edge Detection", New EdgeDetectionCommand(EdgeDetectionType.Gradient, 1))
        End Sub

        Private Sub menuFiltersCannyEdgeDetector_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersCannyEdgeDetector.Click
            ShowCommand("Canny Edge Detector", New CannyEdgeDetectorCommand(2.4, 30, 70))
        End Sub

        Private Sub menuFiltersDustScratchRemoval_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFiltersDustScratchRemoval.Click
            ShowCommand("Dust & Scratch Removal", New DustAndScratchRemovalCommand(9, 50, 100))
        End Sub

#End Region

#Region "Transforms Commands"

        Private Sub menuTransformsApply_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsApply.Click
            If transformChain.Count = 0 Then
                MessageBox.Show("You need to add at least one transform to the chain")
            Else
                ShowCommand("Transform Chain", Me.transformChain)
            End If
        End Sub

        Private Sub menuTransformsBumpMap_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsBumpMap.Click
            Dim bumpFile As OpenFileDialog = New OpenFileDialog
            bumpFile.Filter = HelperMethods.CreateDialogFilter(True)
            Dim image As AtalaImage = Nothing
            If bumpFile.ShowDialog() = DialogResult.OK Then
                image = New AtalaImage(bumpFile.FileName)
            End If
            bumpFile.Dispose()
            If image Is Nothing Then
                Return
            End If

            ShowTransformCommand("Bump Map Transform", New BumpMapTransform(1.2, image))
        End Sub

        Private Sub menuTransformsChain_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsChain.Click
            Me.menuTransformsChain.Checked = Not menuTransformsChain.Checked
            Me.chainTransforms = menuTransformsChain.Checked
            Me.menuTransformsApply.Enabled = Me.chainTransforms
        End Sub

        Private Sub menuTransformsElliptical_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsElliptical.Click
            Dim elsize As Integer
            If Viewer.Selection.Visible Then
                Dim region As RectangleF = Me.GetRegionOfInterest().Region.GetBounds(Viewer.Image.GetGraphics())
                If region.Width > region.Height Then
                    elsize = CInt(region.Height)
                Else
                    elsize = CInt(region.Width)
                End If
            Else
                elsize = CInt(Math.Max(Viewer.Image.Width, Viewer.Image.Height))
            End If

            If elsize < 1 Then
                elsize = 200
            End If
            ShowTransformCommand("Elliptcal Transform", New EllipticalTransform(New Size(elsize, elsize), Point.Empty, CompressTransformMode.Both))
        End Sub

        Private Sub menuTransformsLens_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsLens.Click
            ShowTransformCommand("Lens Transform", New LensTransform(400, Point.Empty))
        End Sub

        Private Sub menuLineSlice_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuLineSlice.Click
            ShowTransformCommand("Line Slice Transform", New LineSliceTransform(10, 300, True))
        End Sub

        Private Sub menuTransformsMarble_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsMarble.Click
            ShowTransformCommand("Marble Transform", New MarbleTransform(0.8, New Size(4, 5)))
        End Sub

        Private Sub menuTransformsOffset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsOffset.Click
            ShowTransformCommand("Offset Transform", New OffsetTransform(New Point(60, 20), OffsetTransformMode.WrapBothEdges))
        End Sub

        Private Sub menuTransformsPerlin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsPerlin.Click
            ShowTransformCommand("Perlin Transform", New PerlinTransform(0.8, New Size(4, 5)))
        End Sub

        Private Sub menuTransformsPinch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsPinch.Click
            ShowTransformCommand("Pinch Transform", New PinchTransform(400, 30, Point.Empty))
        End Sub

        Private Sub menuTransformsPolygon_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsPolygon.Click
            Dim pts As Point() = New Point(3) {New Point(30, 20), New Point(100, 56), New Point(200, 140), New Point(135, 168)}
            ShowTransformCommand("Polygon Transform", New PolygonTransform(pts, CompressTransformMode.Both))
        End Sub

        Private Sub menuTransformsRandom_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsRandom.Click
            ShowTransformCommand("Random Transform", New RandomTransform(20))
        End Sub

        Private Sub menuTransformsRipple_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsRipple.Click
            ShowTransformCommand("Ripple Transform", New RippleTransform(200, 3.5, 15, Point.Empty, RippleTransformMode.Cosine))
        End Sub

        Private Sub menuTransformsSpin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsSpin.Click
            ShowTransformCommand("Spin Transform", New SpinTransform(300, 200, Point.Empty, Color.White))
        End Sub

        Private Sub menuTransformSpinWave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformSpinWave.Click
            ShowTransformCommand("Spin Wave Transform", New SpinWaveTransform(300, 200, 20, New Point(40, 60)))
        End Sub

        Private Sub menuTransformsWave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsWave.Click
            ShowTransformCommand("Wave Transform", New WaveTransform(30, 10, WaveTransformMode.LeftToRightSine))
        End Sub

        Private Sub menuTransformsWow_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsWow.Click
            ShowTransformCommand("Wow Transform", New WowTransform(200, True))
        End Sub

        Private Sub menuTransformsZigZag_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsZigZag.Click
            ShowTransformCommand("Zig Zag Transform", New ZigZagTransform(30, 10, False))
        End Sub

        Private Sub menuTransformsUser_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuTransformsUser.Click
            ShowTransformCommand("User Transform", New UserTransform(New UserTransformCallback(AddressOf UserTransformPixel)))
        End Sub
#End Region

#Region "Document Commands"
        Private Sub menuDocumentAutoDeskew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDocumentAutoDeskew.Click
            ShowCommand("Auto Deskew", New AutoDeskewCommand)
        End Sub

        Private Sub menuDocumentMedian_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDocumentMedian.Click
            ShowCommand("Binary Median Filter", New DocumentMedianCommand)
        End Sub

        Private Sub menuDocumentMorphological_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDocumentMorphological.Click, menuBinaryDilation.Click, menuBinaryErosion.Click, menuBinaryOpen.Click, menuBinaryClose.Click, menuBinaryBoundary.Click
            Dim item As MenuItem = CType(sender, MenuItem)
            Dim command As MorphoDocumentCommand = New MorphoDocumentCommand(CType(item.Index, MorphoDocumentMode))
            command.RegionOfInterest = GetRegionOfInterest()
            Viewer.ApplyCommand(command, "Binary Morphological " & item.Text)
            UpdateUndoRedoInfo()
        End Sub

        Private Sub menuDocumentThinning_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDocumentThinning.Click
            ShowCommand("Thinning", New DocumentThinningCommand)
        End Sub

        Private Sub menuDocumentHitOrMiss_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDocumentHitOrMiss.Click
            Dim m1 As IntegerMatrix = New IntegerMatrix(3, 3)
            m1.SetRow(0, 0, 0, 0)
            m1.SetRow(1, 0, 1, 1)
            m1.SetRow(2, 0, 0, 0)
            Dim m2 As IntegerMatrix = New IntegerMatrix(3, 3)
            m2.SetRow(0, 0, 0, 0)
            m2.SetRow(1, 1, 0, 0)
            m2.SetRow(2, 0, 0, 0)
            ShowCommand("Hit or Miss", New DocumentHitOrMissCommand(m1, m2))
        End Sub

        Private Sub menuDocumentDespeckle_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDocumentDespeckle.Click
            ShowCommand("Despeckle", New DocumentDespeckleCommand)
        End Sub

        Private Sub menuThresholdAdaptive_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuThresholdAdaptive.Click
            If Not Me.Viewer.Image Is Nothing Then
                Dim cmd As AdaptiveThresholdCommand = New AdaptiveThresholdCommand
                If Me.Viewer.MouseTool = MouseToolType.Selection AndAlso Me.Viewer.Selection.Visible Then
                    cmd.RegionOfInterest = New RegionOfInterest(Me.Viewer.Selection.Bounds)
                End If

                ShowCommand("Adaptive Threshold", cmd)
            End If
        End Sub

        Private Sub menuThresholdGlobal_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuThresholdGlobal.Click
            If Not Me.Viewer.Image Is Nothing Then
                Dim cmd As GlobalThresholdCommand = New GlobalThresholdCommand
                If Me.Viewer.MouseTool = MouseToolType.Selection AndAlso Me.Viewer.Selection.Visible Then
                    cmd.RegionOfInterest = New RegionOfInterest(Me.Viewer.Selection.Bounds)
                End If

                ShowCommand("Global Threshold", cmd)
            End If
        End Sub

        Private Sub menuThresholdDynamic_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuThresholdDynamic.Click
            If Not Me.Viewer.Image Is Nothing Then
                Dim cmd As DynamicThresholdCommand = New DynamicThresholdCommand
                If Me.Viewer.MouseTool = MouseToolType.Selection AndAlso Me.Viewer.Selection.Visible Then
                    cmd.RegionOfInterest = New RegionOfInterest(Me.Viewer.Selection.Bounds)
                End If

                ShowCommand("Dynamic Threshold", cmd)
            End If
        End Sub

        Private Sub menuBorderRemoval_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuBorderRemoval.Click
            If Not Me.Viewer.Image Is Nothing Then
                Dim cmd As BorderRemovalCommand = New BorderRemovalCommand(BorderRemovalEdges.AllSides, 5, False)
                ShowCommand("Border Removal", cmd)
            End If
        End Sub

        Private Sub menuDithering_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuDithering.Click
            If Not Me.Viewer.Image Is Nothing Then
                Dim cmd As DitherCommand = New DitherCommand()
                ShowCommand("Dither", cmd)
            End If
        End Sub
#End Region

#Region "Fft Commands"

        Private Sub menuFftBandPass_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFftBandPass.Click
            Me.Viewer.ApplyCommand(New ResizeCanvasCommand(New Size(FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Height)), Point.Empty, 0))
            ShowCommand("FFT Band Pass Filter", New BandPassFftCommand)
        End Sub

        Private Sub menuFftButterworthHighBoost_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFftButterworthHighBoost.Click
            Me.Viewer.ApplyCommand(New ResizeCanvasCommand(New Size(FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Height)), Point.Empty, 0))
            ShowCommand("FFT Butterworth High Boost Filter", New ButterworthHighBoostFftCommand)
        End Sub

        Private Sub menuFftButterworthHighPass_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFftButterworthHighPass.Click
            Me.Viewer.ApplyCommand(New ResizeCanvasCommand(New Size(FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Height)), Point.Empty, 0))
            ShowCommand("FFT Butterworth High Pass Filter", New ButterworthHighPassFftCommand)

        End Sub

        Private Sub menuFftButterworthLowPass_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFftButterworthLowPass.Click
            Me.Viewer.ApplyCommand(New ResizeCanvasCommand(New Size(FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Height)), Point.Empty, 0))
            ShowCommand("FFT Butterworth Low Pass Filter", New ButterworthLowPassFftCommand)

        End Sub

        Private Sub menuFftGaussianHighBoost_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFftGaussianHighBoost.Click
            Me.Viewer.ApplyCommand(New ResizeCanvasCommand(New Size(FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Height)), Point.Empty, 0))
            ShowCommand("FFT Gaussian High Boost Filter", New GaussianHighBoostFftCommand)

        End Sub

        Private Sub menuFftGaussianHighPass_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFftGaussianHighPass.Click
            Me.Viewer.ApplyCommand(New ResizeCanvasCommand(New Size(FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Height)), Point.Empty, 0))
            ShowCommand("FFT Gaussian High Pass Filter", New GaussianHighPassFftCommand)

        End Sub

        Private Sub menuFftGaussianLowPass_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFftGaussianLowPass.Click
            Me.Viewer.ApplyCommand(New ResizeCanvasCommand(New Size(FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Height)), Point.Empty, 0))
            ShowCommand("FFT Gaussian Low Pass Filter", New GaussianLowPassFftCommand)

        End Sub

        Private Sub menuFftIdealHighPass_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFftIdealHighPass.Click
            Me.Viewer.ApplyCommand(New ResizeCanvasCommand(New Size(FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Height)), Point.Empty, 0))
            ShowCommand("FFT Ideal High Pass Filter", New IdealHighPassFftCommand)

        End Sub

        Private Sub menuFftIdealLowPass_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFftIdealLowPass.Click
            Me.Viewer.ApplyCommand(New ResizeCanvasCommand(New Size(FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Height)), Point.Empty, 0))
            ShowCommand("FFT Ideal Low Pass Filter", New IdealLowPassFftCommand)

        End Sub

        Private Sub menuFftInversePower_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFftInversePower.Click
            Me.Viewer.ApplyCommand(New ResizeCanvasCommand(New Size(FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Width), FftGrid.GetNextPowerOfTwo(Me.Viewer.Image.Height)), Point.Empty, 0))
            ShowCommand("FFT Inverse Power Filter", New InversePowerFftCommand)

        End Sub
#End Region

#Region "Misc Commands"
        Private Sub menuFlipHorizontal_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFlipHorizontal.Click
            Me.Viewer.ApplyCommand(New FlipCommand(FlipDirection.Horizontal), "Flip Horizontal")
        End Sub

        Private Sub menuFlipVertical_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuFlipVertical.Click
            Me.Viewer.ApplyCommand(New FlipCommand(FlipDirection.Vertical), "Flip Vertical")
        End Sub

        Private Sub menuRotate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuRotate.Click
            ' If the image has mixed X and Y resolution values, we need
            ' to correct the values and scale the image.
            Dim resolution As Dpi = Me.Viewer.Image.Resolution
            If resolution.X <> resolution.Y Then
                Dim ratio As Single = CSng(resolution.X) / CSng(resolution.Y)
                Me.Viewer.Image.Resolution = New Dpi(resolution.X, resolution.X, resolution.Units)

                Dim cmd As ImageCommand = Nothing
                If Me.Viewer.Image.PixelFormat = PixelFormat.Pixel1bppIndexed AndAlso AtalaImage.Edition = LicenseEdition.Document Then
                    cmd = New ResampleDocumentCommand(Rectangle.Empty, New Size(Me.Viewer.Image.Width, Convert.ToInt32(Me.Viewer.Image.Height * ratio)), ResampleMethod.Default)
                Else
                    cmd = New ResampleCommand(New Size(Me.Viewer.Image.Width, Convert.ToInt32(Me.Viewer.Image.Height * ratio)), ResampleMethod.Default)
                End If

                Me.Viewer.ApplyCommand(cmd)
            End If

            ShowCommand("Rotate", New RotateCommand(90, Color.White))
        End Sub

        Private Sub menuResizeCanvas_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuResizeCanvas.Click
            Dim canvas As ResizeCanvasCommand = New ResizeCanvasCommand(Viewer.Image.Size, New Point(0, 0), Color.White)
            If Not Viewer.Image.Palette Is Nothing Then
                canvas.CanvasPaletteIndex = Viewer.Image.Palette.GetClosestPaletteIndex(Color.White)
            End If
            ShowCommand("Resize Canvas", canvas)
        End Sub

        Private Sub menuResample_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuResample.Click
            Dim srcRect As Rectangle = New Rectangle(New Point(0, 0), Viewer.Image.Size)
            Dim command As ImageCommand = Nothing

            If Viewer.Image.PixelFormat = PixelFormat.Pixel1bppIndexed Then
                Dim docResample As ResampleDocumentCommand = New ResampleDocumentCommand(srcRect, Viewer.Image.Size, ResampleDocumentMethod.AreaAverage)
                command = CType(docResample, ImageCommand)
            Else
                Dim resample As ResampleCommand = New ResampleCommand(Viewer.Image.Size, ResampleMethod.BiLinear)
                command = CType(resample, ImageCommand)
            End If
            ShowCommand("Resample", command)
        End Sub

        Private Sub menuCrop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuCrop.Click
            ' If there is no selection, display the parameters dialog.
            Dim crop As CropCommand = Nothing
            If Viewer.Selection.Visible Then
                crop = New CropCommand(Viewer.Selection.Bounds)
            Else
                crop = New CropCommand(New Rectangle(New Point(0, 0), Viewer.Image.Size))
                Dim frm As Form = New Parameters("Crop", crop)
                If frm.ShowDialog() <> DialogResult.OK Then
                    frm.Dispose()
                    Return
                End If
                frm.Dispose()
            End If
            Viewer.ApplyCommand(crop, "Crop")
            UpdateUndoRedoInfo()
            Me.Viewer.Selection.Visible = False
        End Sub

        Private Sub menuAutoCrop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuAutoCrop.Click
            ShowCommand("Auto Crop", New AutoCropCommand)
        End Sub

        Private Sub menuSkew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuSkew.Click
            ShowCommand("Skew", New SkewCommand(SkewDirection.Horizontal, 30))
        End Sub

        Private Sub menuQuadrilateralWarp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuQuadrilateralWarp.Click
            ShowCommand("Quadrilateral Warp", New QuadrilateralWarpCommand(New Point(20, 200), New Point(40, 20), New Point(200, 20), New Point(160, 140), InterpolationMode.BiLinear, Color.White))
        End Sub

        Private Sub menuPush_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuPush.Click
            ShowCommand("Push", New PushCommand(New Point(0, 0), New Point(0, 0)))
        End Sub

        Private Sub menuOverlay_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuOverlay.Click, menuOverlayNormal.Click, menuOverlayMasked.Click, menuOverlayMerged.Click
            Dim item As MenuItem = CType(sender, MenuItem)
            Dim caption As String = ""

            ' We need a file before we can create an OverlayCommand object.
            Dim openFile As OpenFileDialog = New OpenFileDialog
            openFile.Filter = HelperMethods.CreateDialogFilter(True)
            openFile.Title = "Select a file to overlay"

            Dim image As AtalaImage = Nothing
            If openFile.ShowDialog() = DialogResult.OK Then
                image = New AtalaImage(openFile.FileName)
            End If

            If image Is Nothing Then
                Return
            End If

            Dim command As ImageCommand = Nothing

            Select Case item.Index
                Case 0
                    command = New OverlayCommand(image, New Point(60, 60), 1)
                    caption = "Overlay"

                Case 1
                    ' We still need the mask image.
                    openFile.Title = "Select the alpha mask"
                    Dim image2 As AtalaImage = Nothing
                    If openFile.ShowDialog() = DialogResult.OK Then
                        image2 = New AtalaImage(openFile.FileName)
                    End If
                    command = New OverlayMaskedCommand(image, image2)
                    caption = "Overlay Masked"

                Case 2
                    command = New OverlayMergedCommand(image, New Point(60, 60), MergeOption.FastBlend, 1)
                    caption = "Overlay Merged"
            End Select
            openFile.Dispose()
            ShowCommand(caption, command)
        End Sub
#End Region

#Region "Draw Menu"
        Private Sub menuDrawLine_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDrawLine.Click
            ClearMouseTools()
            Dim linePen As DrawOutlineParameters = New DrawOutlineParameters
            Dim frm As Form = New Parameters("Line", linePen)
            If frm.ShowDialog() = DialogResult.OK Then
                Me.drawMode = DrawMenuMode.Line
                Me.lineDraw.Parent = Me.Viewer
                Me.lineDraw.Pen = linePen.GetPen()
                Me.canvasSmoothing = linePen.SmoothingLevel
                Me.lineDraw.Active = True
                Viewer.Undos.Add("Draw Line", True)
            End If

            frm.Dispose()
        End Sub

        Private Sub menuDrawLines_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDrawLines.Click
            ClearMouseTools()
            Dim lines As DrawOutlineParameters = New DrawOutlineParameters
            Dim frm As Form = New Parameters("Lines", lines)
            If frm.ShowDialog() = DialogResult.OK Then
                Me.drawMode = DrawMenuMode.Lines
                Me.lineDraw.Pen = lines.GetPen()
                Me.canvasSmoothing = lines.SmoothingLevel
                Me.lineDraw.Active = True
                Viewer.Undos.Add("Draw Lines", True)
            End If
            frm.Dispose()
        End Sub

        Private Sub menuDrawRectangle_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDrawRectangle.Click
            ClearMouseTools()
            Dim rc As RectangleParameters = New RectangleParameters
            Dim frm As Form = New Parameters("Rectangle", rc)
            If frm.ShowDialog() = DialogResult.OK Then
                Viewer.Undos.Add("Draw Rectangle", True)
                Me.canvasSmoothing = rc.SmoothingLevel
                Me.rectangleDraw.Pen = rc.GetPen()
                Me.rectangleDraw.CornerRadius = rc.Rounding
                Me.rectangleDraw.Active = True
                Me.rectangleDraw.Fill = rc.GetFill()
                Me.drawMode = DrawMenuMode.Rectangle
            End If
            frm.Dispose()
        End Sub

        Private Sub menuDrawEllipse_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDrawEllipse.Click
            ClearMouseTools()
            Dim ellipse As DrawSolidParameters = New DrawSolidParameters
            ellipse.Fill.Color = Color.Red
            Dim frm As Form = New Parameters("Ellipse", ellipse)
            If frm.ShowDialog() = DialogResult.OK Then
                Viewer.Undos.Add("Draw Ellipse", True)
                Me.canvasSmoothing = ellipse.SmoothingLevel
                Me.ellipseDraw.Pen = ellipse.GetPen()
                Me.ellipseDraw.Fill = ellipse.GetFill()
                Me.ellipseDraw.Active = True
                Me.drawMode = DrawMenuMode.Ellipse
            End If
            frm.Dispose()
        End Sub

        Private Sub menuDrawPolygon_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDrawPolygon.Click
            ClearMouseTools()
            Dim poly As DrawSolidParameters = New DrawSolidParameters
            Dim frm As Form = New Parameters("Polygon", poly)
            If frm.ShowDialog() = DialogResult.OK Then
                Me.polygonPoints = Nothing
                Viewer.Undos.Add("Draw Polygon", True)
                Me.canvasSmoothing = poly.SmoothingLevel
                Me.lineDraw.Pen = poly.GetPen()
                Me.rectangleDraw.Fill = poly.GetFill()
                Me.lineDraw.Active = True
                Me.drawMode = DrawMenuMode.Polygon
            End If
            frm.Dispose()
        End Sub

        Private Sub menuDrawText_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDrawText.Click
            ClearMouseTools()
            Dim myCanvas As Canvas = New Canvas(Viewer.Image)
            Dim text As TextParameters = New TextParameters
            Dim frm As Form = New Parameters("Text", text)
            If frm.ShowDialog() = DialogResult.OK Then
                Viewer.Undos.Add("Draw Text", True)
                myCanvas.SmoothingLevel = text.SmoothingLevel
                myCanvas.FontQuality = text.FontQuality
                Dim tf As TextFormat = text.GetTextFormat()

                If Viewer.Selection.Visible AndAlso (Viewer.Selection.GetType() Is GetType(RectangleSelection)) Then
                    myCanvas.DrawText(text.Text, Viewer.Selection.Bounds, text.Font, New SolidFill(text.Color), New SolidFill(Me.currentTextBackColor), tf)
                    Viewer.Selection.Visible = False
                Else
                    myCanvas.DrawText(text.Text, text.Position, text.Font, New SolidFill(text.Color), New SolidFill(Me.currentTextBackColor), tf)
                End If

                Viewer.Refresh()
            End If
            frm.Dispose()
        End Sub

        Private Sub menuDrawSetBackcolor_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDrawSetBackcolor.Click
            Dim clr As ColorDialog = New ColorDialog
            clr.Color = Me.currentTextBackColor
            If clr.ShowDialog() = DialogResult.OK Then
                Me.currentTextBackColor = clr.Color
            End If
            clr.Dispose()
        End Sub

        Private Sub menuDrawClearBackcolor_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDrawClearBackcolor.Click
            Me.currentTextBackColor = Color.Empty
        End Sub


        Private Sub menuDrawFreehand_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDrawFreehand.Click
            ClearMouseTools()
            Dim freehand As DrawOutlineParameters = New DrawOutlineParameters
            Dim frm As Form = New Parameters("Freehand", freehand)
            If frm.ShowDialog() = DialogResult.OK Then
                Me.drawMode = DrawMenuMode.Freehand
                Me.lineDraw.Pen = freehand.GetPen()
                Me.canvasSmoothing = freehand.SmoothingLevel
                Viewer.Undos.Add("Freehand", True)
            End If
            frm.Dispose()
        End Sub
#End Region

#Region "Help Menu"

        Private Sub menuItem15_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItem15.Click
            Dim aboutBox As AtalaDemos.AboutBox.About = New AtalaDemos.AboutBox.About("About Atalasoft DotImage WinForms Demo", "DotImage WinForms Demo")
            aboutBox.Description = "The most comprehensive of all the demos demonstrating most of the image processing commands and codecs.  This is a good place to learn about the UI features that dotImage offers, as well as testing of image effects, and transforms."
            aboutBox.ShowDialog()
        End Sub
#End Region

#Region "Drag Drop Code"

        Private Sub Viewer_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Viewer.DragOver
            ' Allow files to be dropped onto the control.
            If e.Data.GetDataPresent("FileDrop") OrElse e.Data.GetDataPresent("FileNameW") OrElse e.Data.GetDataPresent("FileName") Then
                e.Effect = DragDropEffects.Link
            Else
                e.Effect = DragDropEffects.None
            End If
        End Sub

        Private Sub Viewer_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Viewer.DragDrop
            Dim dropped As Object = Nothing

            If e.Data.GetDataPresent("FileDrop") Then
                dropped = e.Data.GetData("FileDrop")
            ElseIf e.Data.GetDataPresent("FileNameW") Then
                dropped = e.Data.GetData("FileNameW")
            ElseIf e.Data.GetDataPresent("FileName") Then
                dropped = e.Data.GetData("FileName")
            End If

            If dropped Is Nothing Then
                Return
            End If

            Dim files As String() = CType(IIf(TypeOf dropped Is String, dropped, Nothing), String())
            If files Is Nothing Then
                Return
            End If

            Dim firstImage As Boolean = True

            ' This could be extended to load multipage TIFFs.
            For Each file As String In files
                ' Make sure it's a file we can support.
                Dim fs As FileStream = New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)
                Try
                    Dim decoder As ImageDecoder = RegisteredDecoders.GetDecoder(fs)
                    If decoder Is Nothing Then
                        GoTo Continue1
                    End If

                    ' Wait until we have an image to load before clearing the control.
                    If firstImage Then
                        firstImage = False
                        Me.Viewer.Images.Clear()
                        _images.Clear()
                    End If

                    fs.Seek(0, SeekOrigin.Begin)
                    Dim multi As MultiFramedImageDecoder = CType(IIf(TypeOf decoder Is MultiFramedImageDecoder, decoder, Nothing), MultiFramedImageDecoder)
                    If Not multi Is Nothing Then
                        Dim count As Integer = multi.GetFrameCount(fs)
                        If count > 1 Then
                            If MessageBox.Show(Me, "The file '" & Path.GetFileName(file) & "' contains multiple frames." & Constants.vbCrLf & "Do you want to load all of them?", "Multipage File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                                Dim i As Integer = 0
                                'ORIGINAL LINE: for (int i = 0; i < count; i += 1)
                                'INSTANT VB NOTE: This 'for' loop was translated to a VB 'Do While' loop:
                                Do While i < count
                                    _images.Add(New ImageFileLoadData(file, i))
                                    i += 1
                                Loop
                            Else
                                _images.Add(New ImageFileLoadData(file, 0))
                            End If
                        Else
                            _images.Add(New ImageFileLoadData(file, 0))
                        End If
                    Else
                        _images.Add(New ImageFileLoadData(file, 0))
                    End If
                Catch ex As System.Exception
                    MessageBox.Show(Me, "Error loading " & file & Constants.vbCrLf & ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    fs.Close()
                End Try
Continue1:
            Next file

            If _images.Count > 0 Then
                Dim data As ImageFileLoadData = CType(_images(0), ImageFileLoadData)
                Me.Viewer.Images.Add(New AtalaImage(data.FileName, data.FrameIndex, Nothing))
            End If

            UpdateUndoRedoInfo()
            EnableMenuItems()
            Me.statusBarLoadTime.Text = ""
        End Sub

#End Region



    End Class

    Public Structure ImageFileLoadData
        Public FileName As String
        Public FrameIndex As Integer

        'INSTANT VB NOTE: The parameter fileName was renamed since Visual Basic will not uniquely identify class members when parameters have the same name:
        'INSTANT VB NOTE: The parameter frameIndex was renamed since Visual Basic will not uniquely identify class members when parameters have the same name:
        Public Sub New(ByVal fileName_Renamed As String, ByVal frameIndex_Renamed As Integer)
            Me.FileName = fileName_Renamed
            Me.FrameIndex = frameIndex_Renamed
        End Sub
    End Structure
End Namespace
