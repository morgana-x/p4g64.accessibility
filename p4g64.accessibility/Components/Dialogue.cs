﻿using System.Runtime.InteropServices;
using System.Text;
using DavyKager;
using Reloaded.Hooks.Definitions;
using static p4g64.accessibility.Native.Text.Text;
using static p4g64.accessibility.Utils;

namespace p4g64.accessibility.Components;

/// <summary>
/// A class containing hooks to read out dialogue message
/// </summary>
internal unsafe class Dialogue
{
    private IHook<DrawDialogDelegate> _drawDialogHook;

    private DialogExecutionInfo* _lastDialog = (DialogExecutionInfo*)0;
    private int _lastPage = -1;
    private short _lastSelected = -1;
    private TextStruct* _lastSpeaker;
    private DialogExecution* _playedDialog;
    private IHook<StartDialogDelegate> _startDialogHook;

    internal Dialogue(IReloadedHooks hooks)
    {
        //Debugger.Launch();
        SigScan(
            "48 89 5C 24 ?? 48 89 6C 24 ?? 57 48 83 EC 20 48 8B D9 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 0F B7 05 ?? ?? ?? ??",
            "MsgWindow::DrawDialog",
            address => { _drawDialogHook = hooks.CreateHook<DrawDialogDelegate>(DrawDialog, address).Activate(); });

        SigScan("48 89 5C 24 ?? 55 56 57 41 55 41 57 48 83 EC 40", "MsgWindow::StartDialog", address =>
        {
            _playedDialog = (DialogExecution*)GetGlobalAddress(address + 0x28);
            LogDebug($"Found PlayedDialog at 0x{(nuint)_playedDialog:X}");
            _startDialogHook = hooks.CreateHook<StartDialogDelegate>(StartDialog, address).Activate();
        });
    }

    private uint StartDialog(int executionId, int messageId)
    {
        var res = _startDialogHook.OriginalFunction(executionId, messageId);

        // If we're starting the last dialog we looked at again clear it so the screen reader outputs again
        // (We could probably not check and just always clear when this is called, not 100% sure)
        DialogExecutionInfo* dialog = _playedDialog[executionId].Info;
        LogDebug($"Starting dialog 0x{(nuint)dialog:X}");
        if (dialog == _lastDialog)
        {
            _lastDialog = (DialogExecutionInfo*)0;
        }

        return res;
    }

    private uint DrawDialog(DialogExecutionInfo* dialogInfo)
    {
        var res = _drawDialogHook.OriginalFunction(dialogInfo);

        if (dialogInfo->DialogText != (DialogExecutionInfo*)0)
            SpeakMessage(dialogInfo);

        if (dialogInfo->SelectionText != (DialogExecutionInfo*)0)
            SpeakSelection(dialogInfo);

        return res;
    }

    private void SpeakMessage(DialogExecutionInfo* dialogInfo)
    {
        // Only speak out each bit of dialog once
        if (_lastDialog == dialogInfo &&
            (dialogInfo->CurrentPage == _lastPage || dialogInfo->CurrentPage == dialogInfo->PageCount))
            return;

        LogDebug($"Current page is {dialogInfo->CurrentPage} and last was {_lastPage}");
        LogDebug($"Number of pages is {dialogInfo->PageCount}");
        LogDebug($"Current dialog info is at 0x{(nuint)dialogInfo:x} and last was at 0x{_lastPage:X}");
        StringBuilder sb = new();
        var speakerName = dialogInfo->SpeakerNameText;
        if (speakerName != null && (_lastDialog != dialogInfo || speakerName != _lastSpeaker))
        {
            var speakerNameStr = dialogInfo->SpeakerNameText->ToString();
            if (!string.IsNullOrWhiteSpace(speakerNameStr))
            {
                sb.Append(speakerNameStr + ": ");
            }
        }

        sb.Append(dialogInfo->DialogText->ToString());
        var text = SanitiseDialog(sb.ToString());

        if (!string.IsNullOrWhiteSpace(text))
        {
            LogDebug($"Outputting dialog \"{text}\"");
            Tolk.Output(text, true);
        }

        _lastDialog = dialogInfo;
        _lastPage = _lastDialog->CurrentPage;
        _lastSpeaker = speakerName;
    }

    private void SpeakSelection(DialogExecutionInfo* dialogInfo)
    {
        // Only speak out the current selection once
        if (_lastDialog == dialogInfo && dialogInfo->SelectedOption == _lastSelected)
            return;

        var selectedOption = dialogInfo->SelectedOption;
        if (selectedOption == -1)
        {
            _lastSelected = selectedOption;
            return;
        }

        var text = dialogInfo->SelectionText->GetSelection(selectedOption);
        if (!string.IsNullOrWhiteSpace(text))
        {
            LogDebug($"Outputting selection \"{text}\"");
            Tolk.Output(text, true);
        }

        _lastSelected = selectedOption;
    }

    /// <summary>
    /// Removes parts of the dialog that shouldn't be spoken such as the "> " at the start of some messages
    /// </summary>
    /// <returns>A sanitised version of the dialog</returns>
    private string SanitiseDialog(string dialog)
    {
        if (dialog.StartsWith(">"))
        {
            dialog = dialog.Substring(1).Trim();
        }

        return dialog;
    }


    [StructLayout(LayoutKind.Explicit, Size = 0x40)]
    internal struct DialogExecution
    {
        [FieldOffset(8)] internal DialogExecutionInfo* Info;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DialogExecutionInfo
    {
        [FieldOffset(0x20)] internal TextStruct* SpeakerNameText;

        [FieldOffset(0x38)] internal TextStruct* DialogText;

        [FieldOffset(0x70)] internal TextStruct* SelectionText;

        [FieldOffset(0x7e)] internal short SelectedOption;

        [FieldOffset(0x80)] internal short LastSelectedOption;

        [FieldOffset(0x82)] internal short NumSelectionOptions;

        [FieldOffset(0x48)] internal short CurrentPage;

        [FieldOffset(0x4a)] internal short PageCount;
    }

    private delegate uint DrawDialogDelegate(DialogExecutionInfo* dialogInfo);

    private delegate uint StartDialogDelegate(int executionId, int messageId);
}