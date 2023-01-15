Imports System.ComponentModel

Partial Class PageControlDesigner
    Public Class DesignerControl
        Inherits Control
        Private Shadows Parent As PageControlDesigner
        Private Items As New List(Of Item)

        Sub New(Parent As PageControlDesigner)
            Me.Parent = Parent
            AllowDrop = True

			For Each page As PageControl.Page In Parent.GetControl.Pages
				AddItem(page)
			Next
        End Sub

        Public Sub AddItem(Page As IComponent)
            Dim itm As New Item
            itm.Page = Page
            If itm.Page.PageIndex = -1 Then itm.Page.PageIndex = Items.Count + 1

            Items.Add(itm)
            Invalidate()
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            With e.Graphics
                For i = 0 To Items.Count - 1
                    Dim itm As Item = Items(i)

                    If i = SelectionIndex Then
                        itm.Draw(e.Graphics, SelPos.X, SelPos.Y)
                    Else
                        itm.Draw(e.Graphics, itm.GetDrawPoint.X, itm.GetDrawPoint.Y)
                    End If
                Next

                For i = 0 To Width Step +100
                    .DrawLine(Pens.Black, i, 0, i, Height)
                    .DrawLine(Pens.Black, 0, i, Width, i)
                Next
            End With
        End Sub

#Region " Drag support "
        Private SelectionIndex As Integer = -1
        Private SelPos, SelOffset As Point

        Private Sub DesignerControl_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
            If Not e.Button = Windows.Forms.MouseButtons.Left Then Exit Sub

            SelectionIndex = GetItemFromPoint(e.Location)
            If SelectionIndex = -1 Then Exit Sub

            SelOffset = New Point(e.X - Items(SelectionIndex).GetDrawPoint.X, e.Y - Items(SelectionIndex).GetDrawPoint.Y)
        End Sub

        Private Sub DesignerControl_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
            If Not e.Button = Windows.Forms.MouseButtons.Left Or SelectionIndex = -1 Then Exit Sub

            Dim OldPos As Point = SelPos
            SelPos = e.Location - SelOffset

            Dim invalRect = PointsToRect(OldPos, SelPos)
            invalRect.Width += 100 : invalRect.Height += 100
            Invalidate(invalRect)
        End Sub

        Private Sub DesignerControl_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
            If Not e.Button = Windows.Forms.MouseButtons.Left Or SelectionIndex = -1 Then Exit Sub
            Dim cp = GetClosestPoint(SelPos)

            If GetItemFromPoint(cp) = -1 Then
                SetValue(Items(SelectionIndex), "PageLocation", New Point(cp.X / 100, cp.Y / 100))
            End If

            SelectionIndex = -1
            Invalidate()
        End Sub

        Private Function GetItemFromPoint(p As Point) As Integer
            Dim mr As New Rectangle(p, New Size(1, 1))

            For i = 0 To Items.Count - 1
                Dim p2 = Items(i).GetDrawPoint
                If New Rectangle(p2.X, p2.Y, 100, 100).IntersectsWith(mr) Then Return i
            Next

            Return -1
        End Function

        Private Function PointsToRect(p1 As Point, p2 As Point) As Rectangle
            PointsToRect.X = Math.Min(p1.X, p2.X)
            PointsToRect.Y = Math.Min(p1.Y, p2.Y)
            PointsToRect.Width = Math.Max(p1.X, p2.X) - PointsToRect.X
            PointsToRect.Height = Math.Max(p1.Y, p2.Y) - PointsToRect.Y
        End Function

        Private Function GetClosestPoint(p As Point) As Point
            Dim dis = Function(p1 As Point, p2 As Point) (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)
            Dim flr = Function(i%) CInt(Math.Floor(i / 100)) * 100
            Dim pts As Point() = {New Point(flr(p.X), flr(p.Y)), New Point(flr(p.X) + 100, flr(p.Y)), New Point(flr(p.X), flr(p.Y) + 100), New Point(flr(p.X) + 100, flr(p.Y) + 100)}
            Dim d = Width * Height, index = -1
            Dim pIndex = -1

            For i = 0 To 3
                Dim p2 = pts(i)
                Dim d2 = dis(p, p2)

                If d2 < d Then
                    d = d2
                    pIndex = i
                End If
            Next

            Return pts(pIndex)
        End Function
#End Region

        Private Structure Item
            Public Page As PageControl.Page

            Private B As Bitmap
            Private Function GetBitmap() As Bitmap
                If B Is Nothing Then
                    B = New Bitmap(100, 100)

                    Using G As Graphics = Graphics.FromImage(B)
                        G.FillRectangle(Brushes.Red, 0, 0, 100, 100)
                        G.DrawString(Page.Name, New Font("Verdana", 11), Brushes.Black, 0, 0)
                        G.DrawImageUnscaled(CreateIndexBall(20, Page.PageIndex), 75, 75)
                    End Using
                End If

                Return B
            End Function

            Public Sub Draw(G As Graphics, x%, y%)
                G.DrawImageUnscaled(GetBitmap, x, y)
            End Sub

            Public Function GetDrawPoint() As Point
                Return New Point(Page.PageLocation.X * 100, Page.PageLocation.Y * 100)
            End Function

            Public Sub Dispose()
                B.Dispose()
                Page.Dispose()
            End Sub

            Private Function CreateIndexBall(w%, i%) As Bitmap
                Dim B As New Bitmap(w, w)
                Dim G As Graphics = Graphics.FromImage(B)
                Dim r As RectangleF

                Using pth As New Drawing2D.GraphicsPath
                    pth.AddString(i.ToString, New FontFamily("Verdana"), 0, 20, New Point, StringFormat.GenericDefault)
                    r = pth.GetBounds

                    pth.Transform(New Drawing2D.Matrix(1, 0, 0, 1, -r.X, -r.Y))
                    pth.Transform(New Drawing2D.Matrix(r.Height / (w - 3), 0, 0, r.Height / (w - 3), 0, 0))
                    r = pth.GetBounds
                    pth.Transform(New Drawing2D.Matrix(1, 0, 0, 1, (w - r.Width - 1) / 2, (w - r.Height - 1) / 2))

                    G.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                    G.FillEllipse(Brushes.Blue, 0, 0, w - 1, w - 1)

                    G.FillPath(Brushes.White, pth)
                End Using

                G.Dispose()
                Return B
            End Function
        End Structure

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                For Each i As Item In Items
                    i.Dispose()
                Next
            End If

            MyBase.Dispose(disposing)
        End Sub

        ' The controldesigner don't allways save properties which are changed directly :/
        Private Sub SetValue(itm As Item, pName$, val As Object)
            Dim Prop As PropertyDescriptor = TypeDescriptor.GetProperties(itm.Page)(pName)
            If Prop IsNot Nothing AndAlso Prop.PropertyType Is val.GetType Then Prop.SetValue(itm.Page, val)
        End Sub
    End Class
End Class
