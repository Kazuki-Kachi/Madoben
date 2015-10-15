Imports System.Reactive.Linq
Imports ServedWhiteNoodlesFlowingInSmallFlumeLibraries
Imports ServedWhiteNoodlesFlowingInSmallFlumeLibraries.RandomProvider

Module Module1

    Sub Main()

        Dim guests = Enumerable.Range(1, GetThreadRandom().Next(1, 11)).Select(Function(i) New Guest($"ゲスト{i}")).ToArray()
        Dim sv = New Server(1000)

        Observable.
            FromEvent(Of EventHandler(Of NoodleServeEventArg), NoodleServeEventArg)(Function(h) Sub(sender, e) h(e), Sub(h) AddHandler sv.Served, h, Sub(h) RemoveHandler sv.Served, h).
            TakeWhile(Function(i) guests.Any(Function(guest) Not guest.IsSatiety)).
            Subscribe(Sub(e) guests.Aggregate(e.Noodles, Function(noodles, guest)
                                                             Dim pickedCount = guest.Picking(noodles)
                                                             guest.Eat(noodles.Take(pickedCount))
                                                             Return noodles.Skip(pickedCount).ToArray()
                                                         End Function),
                      Sub()
                          WriteLine("全員満腹になりました。")
                          sv.Dispose()
                      End Sub)
        Console.ReadKey()
    End Sub

End Module
