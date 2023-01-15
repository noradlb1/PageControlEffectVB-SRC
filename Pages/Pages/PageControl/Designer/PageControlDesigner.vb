Imports System.Windows.Forms.Design
Imports System.ComponentModel.Design
Imports System.ComponentModel
Imports System.Drawing.Design

<ToolboxItemFilter("", ToolboxItemFilterType.Custom)> _
Public Class PageControlDesigner
    Inherits ControlDesigner
    Implements IRootDesigner, IToolboxUser

    Private Designer As DesignerControl

    Private Function GetView(ByVal technology As ViewTechnology) As Object Implements IRootDesigner.GetView
        If technology <> 1 Then Throw New ArgumentException("technology")

        If Designer Is Nothing Then Designer = New DesignerControl(Me)
        Return Designer
    End Function

    Public ReadOnly Property SupportedTechnologies As ViewTechnology() Implements IRootDesigner.SupportedTechnologies
        Get
            Return New ViewTechnology() {1}
        End Get
    End Property

#Region " Toolbox "
    Private Function GetToolSupported(ByVal item As ToolboxItem) As Boolean Implements IToolboxUser.GetToolSupported
		Return Type.GetType(item.TypeName, False).IsSubclassOf(GetType(PageControl.Page))
    End Function

    ' This is called by the toolbox service when the user double clicks on a toolbox item. 
    ' The item we get has already passed the filter test so we don't need to do any additional checks.
    Private Sub ToolPicked(ByVal item As ToolboxItem) Implements IToolboxUser.ToolPicked
        For Each comp As IComponent In item.CreateComponents(CType(GetService(GetType(IDesignerHost)), IDesignerHost))
			If TypeOf comp Is PageControl.Page Then
				AddItem(comp)
				Designer.AddItem(comp)
			End If
        Next
    End Sub

	Private Sub AddItem(p As PageControl.Page)
		Dim Prop As PropertyDescriptor = TypeDescriptor.GetProperties(GetControl)("Pages")

		Dim oldVal As PageControl.Page() = Prop.GetValue(GetControl)
		Dim newVal(oldVal.Count) As PageControl.Page

		For i = 0 To oldVal.Count - 1
			newVal(i) = oldVal(i)
		Next

		newVal(newVal.Count - 1) = p
		Prop.SetValue(GetControl, newVal)
	End Sub
#End Region

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Designer IsNot Nothing Then Designer.Dispose()
        End If

        MyBase.Dispose(disposing)
    End Sub

    Public Function GetControl() As PageControl
        Return DirectCast(Component, PageControl)
    End Function
End Class
