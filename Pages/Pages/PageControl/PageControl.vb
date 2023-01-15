Imports System.ComponentModel
Imports System.ComponentModel.Design

<Designer(GetType(PageControlDesigner), GetType(IRootDesigner))> _
Public Class PageControl
    Inherits Control

    Private WithEvents Panel As New Panel With {.Dock = DockStyle.Fill, .Parent = Me}
    Private WithEvents Animation As New PageAnimation(Me)

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)

        Dim m = Me.GetType.GetMethod("InitializeComponent", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
        If m IsNot Nothing Then m.Invoke(Me, Nothing)
    End Sub

#Region " Properties "
    Private _Pages As Page() = {}
    Public Property Pages As Page()
        Get
            Return _Pages
        End Get
        Set(value As Page())
            Array.Sort(value, New Page.PageSorter)
            _Pages = value
        End Set
    End Property

    Private _SelectionIndex As Integer = 0
    Public Property SelectionIndex As Integer
        Get
            If Pages.Count = 0 Then Return -1 Else Return _SelectionIndex
        End Get
        Set(value As Integer)
            If value < 0 Or value > Pages.Count - 1 Then Exit Property
            _SelectionIndex = value

            Panel.Controls.Clear()
            Panel.Controls.Add(Pages(value))
        End Set
    End Property
#End Region

#Region " Painting "
    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        ' Overided to improve drawing speed
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim w = Width - 1, h = Height - 1

        With e.Graphics
            .Clear(BackColor)

            If Animation.IsPlaying Then Animation.Draw(e.Graphics)
        End With
    End Sub
#End Region

    Public Sub GoToPage(index%)
        Animation.Start(SelectionIndex, index)
    End Sub

    Private Sub Animation_AnimationStarted() Handles Animation.AnimationStarted
        Panel.Visible = False
    End Sub

    Private Sub Animation_AnimationStopped(TargetFrame%) Handles Animation.AnimationStopped
        SelectionIndex = TargetFrame
        Panel.Visible = True
    End Sub

    Private Sub CenterPanel() Handles Panel.ControlAdded, Panel.SizeChanged
        Dim p As Page = Panel.Controls(0)

        If p.PageCentered Then
            p.Dock = DockStyle.None
            p.Location = New Point((Panel.Width - p.Width) / 2, (Panel.Height - p.Height) / 2)
        Else
            p.Dock = DockStyle.Fill
        End If
    End Sub
End Class
