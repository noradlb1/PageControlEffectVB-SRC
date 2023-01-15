Public Class MyPages : Inherits PageControl
    Friend WithEvents Page31 As Pages.Page3
    Friend WithEvents Page21 As Pages.Page2
    Friend WithEvents Page11 As Pages.Page1

    Private Sub InitializeComponent()
        Me.Page11 = New Pages.Page1()
        Me.Page21 = New Pages.Page2()
        Me.Page31 = New Pages.Page3()
        Me.SuspendLayout()
        '
        'Page11
        '
        Me.Page11.Location = New System.Drawing.Point(-117, -40)
        Me.Page11.Name = "Page11"
        Me.Page11.PageCentered = True
        Me.Page11.PageIndex = 1
        Me.Page11.PageLocation = New System.Drawing.Point(2, 2)
        Me.Page11.Size = New System.Drawing.Size(234, 80)
        Me.Page11.TabIndex = 0
        '
        'Page21
        '
        Me.Page21.Location = New System.Drawing.Point(0, 0)
        Me.Page21.Name = "Page21"
        Me.Page21.PageCentered = True
        Me.Page21.PageIndex = 2
        Me.Page21.PageLocation = New System.Drawing.Point(3, 2)
        Me.Page21.Size = New System.Drawing.Size(126, 16)
        Me.Page21.TabIndex = 0
        '
        'Page31
        '
        Me.Page31.Location = New System.Drawing.Point(0, 0)
        Me.Page31.Name = "Page31"
        Me.Page31.PageCentered = False
        Me.Page31.PageIndex = 3
        Me.Page31.PageLocation = New System.Drawing.Point(2, 4)
        Me.Page31.Size = New System.Drawing.Size(356, 293)
        Me.Page31.TabIndex = 0
        '
        'MyPages
        '
        Me.Name = "MyPages"
        Me.Pages = New Pages.PageControl.Page() {CType(Me.Page11, Pages.PageControl.Page), CType(Me.Page21, Pages.PageControl.Page), CType(Me.Page31, Pages.PageControl.Page)}
        Me.SelectionIndex = 0
        Me.ResumeLayout(False)

    End Sub
End Class