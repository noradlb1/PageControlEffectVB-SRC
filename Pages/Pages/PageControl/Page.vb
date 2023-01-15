Partial Class PageControl
    Public Class Page
        Inherits UserControl

        Public Property PageLocation As Point
        Public Property PageCentered As Boolean
        Public Property PageIndex As Integer = -1

        Public Class PageSorter
            Implements IComparer(Of Page)

            Public Function Compare(ByVal x As Page, ByVal y As Page) As Integer Implements System.Collections.Generic.IComparer(Of Page).Compare
                Return x.PageIndex.CompareTo(y.PageIndex)
            End Function
        End Class
    End Class
End Class
