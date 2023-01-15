Partial Class PageControl
    Private Class PageAnimation
        Private Parent As PageControl, Bitmap As Bitmap, Animation As Animation, Rect As Rectangle, From%, To%
        Private WithEvents Tmr As New Timer With {.Interval = 1, .Enabled = True}

        Public Event AnimationStarted()
        Public Event AnimationStopped(TargetFrame%)

        Sub New(PageControl As PageControl)
            Parent = PageControl
        End Sub

        Public Sub Start(From%, To%)
            If From = [To] Then Exit Sub
            If From < 0 Or From > Parent.Pages.Count - 1 Then Exit Sub
            If [To] < 0 Or [To] > Parent.Pages.Count - 1 Then Exit Sub

            Rect = New Rectangle(New Point(Math.Min(Parent.Pages([From]).PageLocation.X, Parent.Pages([To]).PageLocation.X), Math.Min(Parent.Pages([From]).PageLocation.Y, Parent.Pages([To]).PageLocation.Y)), _
                                 New Size(Math.Max(Parent.Pages([From]).PageLocation.X, Parent.Pages([To]).PageLocation.X), Math.Max(Parent.Pages([From]).PageLocation.Y, Parent.Pages([To]).PageLocation.Y)))
            Rect.Width += 1 - Rect.X : Rect.Height += 1 - Rect.Y

            Me.From = [From] : Me.To = [To]
            If Bitmap IsNot Nothing Then Bitmap.Dispose()
            Bitmap = DrawPagesToBitmap()

            Animation = New Animation(0, 100, 500)
			Animation.Type = Animation.AnimationType.Jumpback
            Animation.StartAnimation()
        End Sub

        Private Function DrawPagesToBitmap() As Bitmap
            Dim B As New Bitmap(CInt(Rect.Width * Parent.Width), CInt(Rect.Height * Parent.Height))

            For i = 0 To Parent.Pages.Count - 1
                If Not Rect.IntersectsWith(New Rectangle(Parent.Pages(i).PageLocation, New Size(1, 1))) Then Continue For
                Dim p = Parent.Pages(i)
                Dim r = New Rectangle((p.PageLocation.X - Rect.X) * Parent.Width + If(p.PageCentered, (Parent.Width - p.Width) / 2, 0), (p.PageLocation.Y - Rect.Y) * Parent.Height + If(p.PageCentered, (Parent.Height - p.Height) / 2, 0), Parent.Width, Parent.Height)

                p.DrawToBitmap(B, r)
            Next

            Return B
        End Function

        Public Sub Draw(G As Graphics)
            Dim fx = Parent.Pages([From]).PageLocation.X - Rect.X
            Dim fy = Parent.Pages([From]).PageLocation.Y - Rect.Y
            Dim tx = Parent.Pages([To]).PageLocation.X - Rect.X
            Dim ty = Parent.Pages([To]).PageLocation.Y - Rect.Y

            Dim x = (fx - tx) * Parent.Width / 100
            Dim y = (fy - ty) * Parent.Height / 100
            Dim f = Animation.GetFrame

            G.DrawImageUnscaled(Bitmap, x * f - If(fx > tx, Parent.Width * (Rect.Width - 1), 0), y * f - If(fy > ty, Parent.Height * (Rect.Height - 1), 0))
        End Sub

        Public Function IsPlaying() As Boolean
            If Animation Is Nothing Then Return False Else Return Not Animation.AnimationEnded
        End Function

        Private Sub Tmr_Tick(sender As Object, e As EventArgs) Handles Tmr.Tick
            Static WherePlaying As Boolean = False
            Dim p = IsPlaying()

            If p <> WherePlaying Then
                If WherePlaying Then RaiseEvent AnimationStopped(Me.To) Else RaiseEvent AnimationStarted()
                WherePlaying = p
            End If

            If p Then Parent.Invalidate()
        End Sub
    End Class
End Class
