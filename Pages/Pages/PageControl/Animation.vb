' Credits goes to http://javascript.info/tutorial/animation for the mathematical formulas

Public Class Animation
    Private Watch As New Stopwatch
    Private ReversePos As Long
    Private ReverseTime As Long

#Region " Properties "
    Public Property Start As Single = 0
    Public Property [Stop] As Single = 100
    Public Property Length As Single = 1500
    Public Property Type As AnimationType = AnimationType.Linear
    Public Property Mode As EaseMode = EaseMode.EaseIn

    Public ReadOnly Property AnimationEnded As Boolean
        Get
            If GetPos() >= Length Or GetPos() <= 0 Then Return True Else Return False
        End Get
    End Property

    Private _PlayForward As Boolean = True
    Public Property PlayForward As Boolean
        Get
            Return _PlayForward
        End Get
        Set(value As Boolean)
            If _PlayForward = value Then Exit Property

            Dim pos = GetPos(), time = Watch.ElapsedMilliseconds
            ReversePos = pos
            ReverseTime = time
            _PlayForward = value
        End Set
    End Property
#End Region

    Sub New()
    End Sub

    Sub New(Lenght!)
        Me.Length = Lenght
    End Sub

    Sub New(Start!, Stop!, Length!)
        Me.Start = Start
        Me.Stop = [Stop]
        Me.Length = Length
    End Sub

    Public Sub StartAnimation()
        If Not Watch.IsRunning Then Watch.Start()
    End Sub
    Public Sub StopAnimation()
        Watch.Stop()
    End Sub
    Public Sub ResetAnimation()
        Watch.Reset()
    End Sub

    Private Function GetPos() As Long
        Dim p = Watch.ElapsedMilliseconds

        If PlayForward Then
            p = ReversePos + (p - ReverseTime)
        Else
            p = ReversePos - (p - ReverseTime)
        End If

        'Console.WriteLine(p & " - " & ReversePos)
        If p > Length Then p = Length
        If p < 0 Then p = 0

        Return p
    End Function

    Public Function GetFrame() As Single
        Return GetFrame(Start, [Stop], Length, GetPos, Type, Mode)
    End Function

    Public Shared Function GetFrame(Start!, Stop!, Length!, Position!, Type As AnimationType, Optional Mode As EaseMode = EaseMode.EaseIn) As Single
        Dim p As Single = Position / Length

        Select Case Mode
            Case Is = EaseMode.EaseIn : GetFrame = CalculateAnimation(p, Type)
            Case Is = EaseMode.EaseOut : GetFrame = 1 - CalculateAnimation(1 - p, Type)
            Case Is = EaseMode.EaseInOut : GetFrame = If(p <= 0.5, CalculateAnimation(2 * p, Type) / 2, (2 - CalculateAnimation(2 * (1 - p), Type)) / 2)
        End Select

        Return ([Stop] - Start) * GetFrame + Start
    End Function

    Private Shared Function CalculateAnimation(Progress As Single, Type As AnimationType) As Single
        Dim p As Single = Progress

        Select Case Type
            Case Is = AnimationType.Linear : Return p
            Case Is = AnimationType.Quadrantic : Return Math.Pow(p, 5)
            Case Is = AnimationType.Circular : Return 1 - Math.Sin(Math.Acos(p))
            Case Is = AnimationType.Jumpback : Return Math.Pow(p, 2) * ((1.5 + 1) * p - 1.5)
            Case Is = AnimationType.Bounce
                Dim a As Double = 0, b As Double = 1

                Do
                    If p >= (7 - 4 * a) / 11 Then Return -Math.Pow((11 - 6 * a - 11 * p) / 4, 2) + Math.Pow(b, 2)
                    a += b : b /= 2
                Loop
        End Select
    End Function

    Public Enum AnimationType
        Linear = 0
        Quadrantic
        Circular
        Jumpback
        Bounce
    End Enum

    Public Enum EaseMode
        EaseIn = 0
        EaseOut
        EaseInOut
    End Enum
End Class
