Imports Microsoft.VisualBasic
Imports System
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.Windows.Forms

Namespace dotImageDemo
	''' <summary>
	''' Summary description for MultiSave.
	''' </summary>
	Public Class MultiSave
		Inherits System.Windows.Forms.Form
		Friend images As ArrayList = New ArrayList()

		Private panel1 As System.Windows.Forms.Panel
		Private btnSave As System.Windows.Forms.Button
		Private btnCancel As System.Windows.Forms.Button
		Friend WithEvents listImages As System.Windows.Forms.ListBox
		Friend propertyGrid1 As System.Windows.Forms.PropertyGrid
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.Container = Nothing

		Public Sub New()
			'
			' Required for Windows Form Designer support
			'
			InitializeComponent()

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
			Me.panel1 = New System.Windows.Forms.Panel()
			Me.btnCancel = New System.Windows.Forms.Button()
			Me.btnSave = New System.Windows.Forms.Button()
			Me.listImages = New System.Windows.Forms.ListBox()
			Me.propertyGrid1 = New System.Windows.Forms.PropertyGrid()
			Me.panel1.SuspendLayout()
			Me.SuspendLayout()
			' 
			' panel1
			' 
			Me.panel1.Controls.AddRange(New System.Windows.Forms.Control() { Me.btnCancel, Me.btnSave})
			Me.panel1.Dock = System.Windows.Forms.DockStyle.Bottom
			Me.panel1.Location = New System.Drawing.Point(0, 212)
			Me.panel1.Name = "panel1"
			Me.panel1.Size = New System.Drawing.Size(427, 54)
			Me.panel1.TabIndex = 0
			' 
			' btnCancel
			' 
			Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.btnCancel.Location = New System.Drawing.Point(258, 11)
			Me.btnCancel.Name = "btnCancel"
			Me.btnCancel.Size = New System.Drawing.Size(96, 30)
			Me.btnCancel.TabIndex = 1
			Me.btnCancel.Text = "Cancel"
			' 
			' btnSave
			' 
			Me.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK
			Me.btnSave.Location = New System.Drawing.Point(72, 11)
			Me.btnSave.Name = "btnSave"
			Me.btnSave.Size = New System.Drawing.Size(96, 30)
			Me.btnSave.TabIndex = 0
			Me.btnSave.Text = "Save"
			' 
			' listImages
			' 
			Me.listImages.Dock = System.Windows.Forms.DockStyle.Left
			Me.listImages.Name = "listImages"
			Me.listImages.Size = New System.Drawing.Size(158, 212)
			Me.listImages.TabIndex = 1
'			Me.listImages.SelectedIndexChanged += New System.EventHandler(Me.listImages_SelectedIndexChanged);
			' 
			' propertyGrid1
			' 
			Me.propertyGrid1.CommandsVisibleIfAvailable = True
			Me.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill
			Me.propertyGrid1.LargeButtons = False
			Me.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar
			Me.propertyGrid1.Location = New System.Drawing.Point(158, 0)
			Me.propertyGrid1.Name = "propertyGrid1"
			Me.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical
			Me.propertyGrid1.Size = New System.Drawing.Size(269, 212)
			Me.propertyGrid1.TabIndex = 2
			Me.propertyGrid1.Text = "propertyGrid1"
			Me.propertyGrid1.ToolbarVisible = False
			Me.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window
			Me.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText
			' 
			' MultiSave
			' 
			Me.AcceptButton = Me.btnSave
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.btnCancel
			Me.ClientSize = New System.Drawing.Size(427, 266)
			Me.Controls.AddRange(New System.Windows.Forms.Control() { Me.propertyGrid1, Me.listImages, Me.panel1})
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
			Me.Name = "MultiSave"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "MultiSave"
			Me.panel1.ResumeLayout(False)
			Me.ResumeLayout(False)

		End Sub
		#End Region

		Private Sub listImages_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles listImages.SelectedIndexChanged
			If listImages.SelectedIndex = -1 Then
			Return
			End If
			Me.propertyGrid1.SelectedObject = Me.images(listImages.SelectedIndex)
		End Sub
	End Class
End Namespace
