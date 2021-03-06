﻿Option Strict On

Namespace Xeora.Web.Controller.Directive
    Public Class PartialCache
        Inherits DirectiveControllerBase
        Implements IParsingRequires
        Implements IInstanceRequires

        Public Event ParseRequested(DraftValue As String, ByRef ContainerController As ControllerBase) Implements IParsingRequires.ParseRequested
        Public Event InstanceRequested(ByRef Instance As [Shared].IDomain) Implements IInstanceRequires.InstanceRequested

        Public Sub New(ByVal DraftStartIndex As Integer, ByVal DraftValue As String, ByVal ContentArguments As [Global].ArgumentInfoCollection)
            MyBase.New(DraftStartIndex, DraftValue, DirectiveTypes.PartialCache, ContentArguments)
        End Sub

        Public Overrides Sub Render(ByRef SenderController As ControllerBase)
            If Me.IsUpdateBlockRequest AndAlso Not Me.InRequestedUpdateBlock Then
                Me.DefineRenderedValue(String.Empty)

                Exit Sub
            End If

            Dim Instance As [Shared].IDomain = Nothing
            RaiseEvent InstanceRequested(Instance)

            Dim UniqueCacheID As String = CacheObject.ProvideUniqueCacheID(Me, Instance)

            If Me.PartialCaches.ContainsKey(Instance.IDAccessTree) AndAlso
                CType(Me.PartialCaches.Item(Instance.IDAccessTree), Hashtable).ContainsKey(UniqueCacheID) Then

                Me.DefineRenderedValue(CType(CType(Me.PartialCaches.Item(Instance.IDAccessTree), Hashtable).Item(UniqueCacheID), CacheObject).Content)
            Else
                Dim ContentDescription As [Global].ContentDescription =
                    New [Global].ContentDescription(Me.InsideValue)

                Dim BlockContent As String =
                    ContentDescription.Parts.Item(0)

                RaiseEvent ParseRequested(BlockContent, Me)

                Dim CacheObject As CacheObject

                Threading.Monitor.Enter(Me.PartialCaches.SyncRoot)
                Try
                    If Not Me.PartialCaches.ContainsKey(Instance.IDAccessTree) Then _
                        Me.PartialCaches.Item(Instance.IDAccessTree) = New Hashtable()

                    Dim WorkingCacheGroup As Hashtable =
                        CType(Me.PartialCaches.Item(Instance.IDAccessTree), Hashtable)

                    If Not WorkingCacheGroup.ContainsKey(UniqueCacheID) Then
                        CacheObject = New CacheObject(UniqueCacheID, Me.Create())

                        WorkingCacheGroup.Item(UniqueCacheID) = CacheObject
                    Else
                        CacheObject = CType(WorkingCacheGroup.Item(UniqueCacheID), CacheObject)
                    End If

                    Me.PartialCaches.Item(Instance.IDAccessTree) = WorkingCacheGroup
                Finally
                    Threading.Monitor.Exit(Me.PartialCaches.SyncRoot)
                End Try

                Me.DefineRenderedValue(CacheObject.Content)
            End If
        End Sub

        Private Shared _PartialCaches As Hashtable = Nothing
        Private ReadOnly Property PartialCaches As Hashtable
            Get
                If PartialCache._PartialCaches Is Nothing Then _
                    PartialCache._PartialCaches = Hashtable.Synchronized(New Hashtable())

                Return PartialCache._PartialCaches
            End Get
        End Property

        Public Shared Sub ClearCache(ByVal DomainIDAccessTree As String())
            If PartialCache._PartialCaches Is Nothing Then Exit Sub

            Threading.Monitor.Enter(PartialCache._PartialCaches.SyncRoot)
            Try
                If PartialCache._PartialCaches.ContainsKey(DomainIDAccessTree) Then _
                    PartialCache._PartialCaches.Remove(DomainIDAccessTree)
            Finally
                Threading.Monitor.Exit(PartialCache._PartialCaches.SyncRoot)
            End Try
        End Sub

        Private Class CacheObject
            Private Shared _PositionRegEx As Text.RegularExpressions.Regex =
                New Text.RegularExpressions.Regex("PC~(?<PositionID>\d+)\:\{", Text.RegularExpressions.RegexOptions.Compiled)
            Private _UniqueCacheID As String

            Private _Content As String
            Private _Date As Date

            Public Sub New(ByVal UniqueCacheID As String, ByVal Content As String)
                Me._UniqueCacheID = UniqueCacheID
                Me._Content = Content
                Me._Date = Date.Now
            End Sub

            Public Shared Function ProvideUniqueCacheID(ByVal PartialCache As PartialCache, ByVal Instance As [Shared].IDomain) As String
                If PartialCache Is Nothing Then Throw New NullReferenceException("PartialCache Parameter must not be null!")

                Dim ServiceFullPath As String = String.Empty

                Dim WorkingObject As ControllerBase =
                    PartialCache.Parent

                Do
                    If TypeOf WorkingObject Is Template Then
                        ServiceFullPath = CType(WorkingObject, Template).ControlID

                        Exit Do
                    End If

                    WorkingObject = WorkingObject.Parent
                Loop Until WorkingObject Is Nothing

                Dim PositionID As Integer = -1
                Dim matchMI As Text.RegularExpressions.Match =
                    CacheObject._PositionRegEx.Match(PartialCache.InsideValue)

                If matchMI.Success Then Integer.TryParse(matchMI.Result("${PositionID}"), PositionID)

                If String.IsNullOrEmpty(Instance.Language.ID) OrElse String.IsNullOrEmpty(ServiceFullPath) OrElse PositionID = -1 Then _
                    Throw New Exception.ParseException()

                Return String.Format("{0}_{1}_{2}", Instance.Language.ID, ServiceFullPath, PositionID)
            End Function

            Public ReadOnly Property UniqueCacheID As String
                Get
                    Return Me._UniqueCacheID
                End Get
            End Property

            Public ReadOnly Property Content As String
                Get
                    Return Me._Content
                End Get
            End Property

            Public ReadOnly Property [Date] As Date
                Get
                    Return Me._Date
                End Get
            End Property
        End Class
    End Class
End Namespace