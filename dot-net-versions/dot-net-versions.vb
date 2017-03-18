Imports System.Collections.Generic
Imports Microsoft.Win32

Module Module1

  Sub Main()

    Console.Write(String.Format("{0}Installed .Net Versions 1 - 4{0}-----{0}", Environment.NewLine))
    DisplayNetVersions(GetInstalledDotNetVersions())

    Console.Write(String.Format("{0}Installed .Net Versions 4.5 and Later{0}-----{0}", Environment.NewLine))
    DisplayNetVersions(Get45PlusDotNetVersions())

    Console.Write(String.Format("{0}.Net Core app are self-contained.{0}", Environment.NewLine))

  End Sub

  ''' <summary>
  ''' Returns the list of installed .Net versions (1-4)
  ''' https://msdn.microsoft.com/en-us/library/hh925568(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-2
  ''' </summary>
  Private Function GetInstalledDotNetVersions() As List(Of String)

    Dim versions As New List(Of String)()

    ' Opens the registry key for the .NET Framework entry.
    Using ndpKey As RegistryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                                  OpenSubKey("SOFTWARE\Microsoft\NET Framework Setup\NDP\")
      ' As an alternative, if you know the computers you will query are running .NET Framework 4.5 
      ' or later, you can use:

      ' As an alternative, if you know the computers you will query are running .NET Framework 4.5 
      ' or later, you can use:
      '  Using ndpKey As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,  _
      '  RegistryView.Registry32).OpenSubKey("SOFTWARE\Microsoft\NET Framework Setup\NDP\")

      For Each versionKeyName As String In ndpKey.GetSubKeyNames()

        If versionKeyName.StartsWith("v") Then

          Dim versionKey As RegistryKey = ndpKey.OpenSubKey(versionKeyName)
          Dim version As String = DirectCast(versionKey.GetValue("Version", ""), String)
          Dim v4 As String = String.Empty
          Dim sp As String = versionKey.GetValue("SP", "").ToString()
          Dim install As String = versionKey.GetValue("Install", "").ToString()

          If install = "" Then
            'no install info, sub version will be added
            v4 = versionKeyName & " " & version
          Else
            If sp <> "" AndAlso install = "1" Then
              versions.Add(versionKeyName & "  " & version & "  SP" & sp)
            End If
          End If

          If version <> "" Then
            Continue For
          End If

          For Each subKeyName As String In versionKey.GetSubKeyNames()

            Dim subKey As RegistryKey = versionKey.OpenSubKey(subKeyName)
            Dim subVersion = DirectCast(subKey.GetValue("Version", ""), String)
            If subVersion <> "" Then
              sp = subKey.GetValue("SP", "").ToString()
            End If
            install = subKey.GetValue("Install", "").ToString()
            If install = "" Then
              'no install info, ust be later
              versions.Add(v4 & " " & versionKeyName & "  " & subVersion)
            Else
              If sp <> "" AndAlso install = "1" Then
                versions.Add(v4 & " " & subKeyName & "  " & subVersion & "  SP" & sp)
              ElseIf install = "1" Then
                versions.Add(v4 & " " & subKeyName & "  " & subVersion)

              End If
            End If
          Next
        End If
      Next
    End Using

    Return versions

  End Function

  ''' <summary>
  ''' Returns the list of installed .Net versions 4.5 and later.
  ''' https://msdn.microsoft.com/en-us/library/hh925568(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-2
  ''' </summary>
  Public Function Get45PlusDotNetVersions() As List(Of String)

    Const subkey As String = "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"

    Dim versions As New List(Of String)()

    Using ndpKey As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey)
      If ndpKey IsNot Nothing AndAlso ndpKey.GetValue("Release") IsNot Nothing Then
        versions.Add(".NET Framework Version: " + CheckFor45PlusVersion(ndpKey.GetValue("Release")))
      Else
        versions.Add(".NET Framework Version 4.5 or later is not detected.")
      End If
    End Using

    Return versions

  End Function

  ''' <summary>
  ''' Checking the version using >= will enable forward compatibility.
  ''' https://msdn.microsoft.com/en-us/library/hh925568(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-2
  ''' </summary>
  Private Function CheckFor45PlusVersion(releaseKey As Integer) As String
    If releaseKey >= 394802 Then
      Return "4.6.2 or later"
    ElseIf releaseKey >= 394254 Then
      Return "4.6.1"
    ElseIf releaseKey >= 393295 Then
      Return "4.6"
    ElseIf releaseKey >= 379893 Then
      Return "4.5.2"
    ElseIf releaseKey >= 378675 Then
      Return "4.5.1"
    ElseIf releaseKey >= 378389 Then
      Return "4.5"
    End If
    ' This code should never execute. A non-null release key should mean
    ' that 4.5 or later is installed.
    Return "No 4.5 or later version detected"
  End Function

  ''' <summary>
  ''' Show the list.
  ''' </summary>
  Private Sub DisplayNetVersions(versions As List(Of String))

    For Each v In versions
      Console.WriteLine(v)
    Next

  End Sub

End Module
