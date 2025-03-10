﻿using BlazorMonaco.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorMonaco.Editor
{
    /**
     * An editor.
     */
    public class Editor : ComponentBase, IDisposable
    {
        #region Blazor

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string CssClass { get; set; }

        [Parameter]
        public EventCallback OnDidDispose { get; set; }

        [Parameter]
        public EventCallback OnDidInit { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        internal DotNetObjectReference<Editor> _dotnetObjectRef;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            JsRuntimeExt.UpdateRuntime(JsRuntime); // Update the static IJSRuntime instance for WASM apps
            _dotnetObjectRef = DotNetObjectReference.Create(this);
        }

        public virtual void Dispose()
        {
            _dotnetObjectRef?.Dispose();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (string.IsNullOrWhiteSpace(Id))
                Id = $"blazor-monaco-{Guid.NewGuid()}";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SetEventListeners();
                await OnDidInit.InvokeAsync(this);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        internal virtual async Task SetEventListeners()
        {
            if (OnDidDispose.HasDelegate)
                await SetEventListener("OnDidDispose");
        }

        internal Task SetEventListener(string eventName)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.setEventListener", Id, eventName);

        public virtual async Task EventCallback(string eventName, string eventJson)
        {
            switch (eventName)
            {
                case "OnDidDispose": await OnDidDispose.InvokeAsync(this); break;
            }
        }

        protected Func<int, string> LineNumbersLambda { get; set; }

        [JSInvokable]
        public string LineNumbersCallback(int lineNumber)
        {
            return LineNumbersLambda?.Invoke(lineNumber) ?? lineNumber.ToString();
        }

        public Task ReloadLineNumbers()
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.reloadLineNumbers", Id);

        #endregion

        /**
         * An event emitted when the editor has been disposed.
         * @event
         */
        //onDidDispose(listener: () => void): IDisposable;
        /**
         * Dispose the editor.
         */
        public Task DisposeEditor()
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.dispose", Id);
        /**
         * Get a unique id for this editor instance.
         */
        //getId(): string;
        /**
         * Get the editor type. Please see `EditorType`.
         * This is to avoid an instanceof check
         */
        public Task<string> GetEditorType()
            => JsRuntime.SafeInvokeAsync<string>("blazorMonaco.editor.getEditorType", Id);
        /**
         * Update the editor's options after the editor has been created.
         */
        // Already implemented in StandaloneCodeEditor and DiffEditor classes
        // updateOptions(newOptions: IEditorOptions): void;

        /**
         * Instructs the editor to remeasure its container. This method should
         * be called when the container of the editor gets resized.
         *
         * If a dimension is passed in, the passed in value will be used.
         *
         * By default, this will also render the editor immediately.
         * If you prefer to delay rendering to the next animation frame, use postponeRendering == true.
         */
        public Task Layout(Dimension dimension = null, bool? postponeRendering = null)
            => JsRuntime.SafeInvokeAsync<string>("blazorMonaco.editor.layout", Id, dimension, postponeRendering);
        /**
         * Brings browser focus to the editor text
         */
        public Task Focus()
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.focus", Id);
        /**
         * Returns true if the text inside this editor is focused (i.e. cursor is blinking).
         */
        public Task<bool> HasTextFocus()
            => JsRuntime.SafeInvokeAsync<bool>("blazorMonaco.editor.hasTextFocus", Id);
        /**
         * Returns all actions associated with this editor.
         */
        //getSupportedActions(): IEditorAction[];
        /**
         * Saves current view state of the editor in a serializable object.
         */
        //saveViewState(): IEditorViewState | null;
        /**
         * Restores the view state of the editor from a serializable object generated by `saveViewState`.
         */
        //restoreViewState(state: IEditorViewState | null): void;
        /**
         * Given a position, returns a column number that takes tab-widths into account.
         */
        public Task<int> GetVisibleColumnFromPosition(Position position)
            => JsRuntime.SafeInvokeAsync<int>("blazorMonaco.editor.getVisibleColumnFromPosition", Id, position);
        /**
         * Returns the primary position of the cursor.
         */
        public Task<Position> GetPosition()
            => JsRuntime.SafeInvokeAsync<Position>("blazorMonaco.editor.getPosition", Id);
        /**
         * Set the primary position of the cursor. This will remove any secondary cursors.
         * @param position New primary cursor's position
         * @param source Source of the call that caused the position
         */
        public Task SetPosition(Position position, string source)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.setPosition", Id, position, source);
        /**
         * Scroll vertically as necessary and reveal a line.
         */
        public Task RevealLine(int lineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLine", Id, lineNumber, scrollType);
        /**
         * Scroll vertically as necessary and reveal a line centered vertically.
         */
        public Task RevealLineInCenter(int lineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLineInCenter", Id, lineNumber, scrollType);
        /**
         * Scroll vertically as necessary and reveal a line centered vertically only if it lies outside the viewport.
         */
        public Task RevealLineInCenterIfOutsideViewport(int lineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLineInCenterIfOutsideViewport", Id, lineNumber, scrollType);
        /**
         * Scroll vertically as necessary and reveal a line close to the top of the viewport,
         * optimized for viewing a code definition.
         */
        //revealLineNearTop(lineNumber: number, scrollType?: ScrollType): void;
        /**
         * Scroll vertically or horizontally as necessary and reveal a position.
         */
        public Task RevealPosition(Position position, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealPosition", Id, position, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a position centered vertically.
         */
        public Task RevealPositionInCenter(Position position, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealPositionInCenter", Id, position, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a position centered vertically only if it lies outside the viewport.
         */
        public Task RevealPositionInCenterIfOutsideViewport(Position position, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealPositionInCenterIfOutsideViewport", Id, position, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a position close to the top of the viewport,
         * optimized for viewing a code definition.
         */
        //revealPositionNearTop(position: IPosition, scrollType?: ScrollType): void;
        /**
         * Returns the primary selection of the editor.
         */
        public Task<Selection> GetSelection()
            => JsRuntime.SafeInvokeAsync<Selection>("blazorMonaco.editor.getSelection", Id);
        /**
         * Returns all the selections of the editor.
         */
        public Task<List<Selection>> GetSelections()
            => JsRuntime.SafeInvokeAsync<List<Selection>>("blazorMonaco.editor.getSelections", Id);
        /**
         * Set the primary selection of the editor. This will remove any secondary cursors.
         * @param selection The new selection
         * @param source Source of the call that caused the selection
         */
        //setSelection(selection: IRange, source?: string): void;
        /**
         * Set the primary selection of the editor. This will remove any secondary cursors.
         * @param selection The new selection
         * @param source Source of the call that caused the selection
         */
        public Task SetSelection(Range selection, string source)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.setSelection", Id, selection, source);
        /**
         * Set the primary selection of the editor. This will remove any secondary cursors.
         * @param selection The new selection
         * @param source Source of the call that caused the selection
         */
        //setSelection(selection: ISelection, source?: string): void;
        /**
         * Set the primary selection of the editor. This will remove any secondary cursors.
         * @param selection The new selection
         * @param source Source of the call that caused the selection
         */
        public Task SetSelection(Selection selection, string source)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.setSelection", Id, selection, source);
        /**
         * Set the selections for all the cursors of the editor.
         * Cursors will be removed or added, as necessary.
         * @param selections The new selection
         * @param source Source of the call that caused the selection
         */
        public Task SetSelections(List<Selection> selections, string source)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.setSelections", Id, selections, source);
        /**
         * Scroll vertically as necessary and reveal lines.
         */
        public Task RevealLines(int startLineNumber, int endLineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLines", Id, startLineNumber, endLineNumber, scrollType);
        /**
         * Scroll vertically as necessary and reveal lines centered vertically.
         */
        public Task RevealLinesInCenter(int lineNumber, int endLineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLinesInCenter", Id, lineNumber, endLineNumber, scrollType);
        /**
         * Scroll vertically as necessary and reveal lines centered vertically only if it lies outside the viewport.
         */
        public Task RevealLinesInCenterIfOutsideViewport(int lineNumber, int endLineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLinesInCenterIfOutsideViewport", Id, lineNumber, endLineNumber, scrollType);
        /**
         * Scroll vertically as necessary and reveal lines close to the top of the viewport,
         * optimized for viewing a code definition.
         */
        //revealLinesNearTop(lineNumber: number, endLineNumber: number, scrollType?: ScrollType): void;
        /**
         * Scroll vertically or horizontally as necessary and reveal a range.
         */
        public Task RevealRange(Range range, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealRange", Id, range, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a range centered vertically.
         */
        public Task RevealRangeInCenter(Range range, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealRangeInCenter", Id, range, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a range at the top of the viewport.
         */
        public Task RevealRangeAtTop(Range range, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealRangeAtTop", Id, range, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a range centered vertically only if it lies outside the viewport.
         */
        public Task RevealRangeInCenterIfOutsideViewport(Range range, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealRangeInCenterIfOutsideViewport", Id, range, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a range close to the top of the viewport,
         * optimized for viewing a code definition.
         */
        //revealRangeNearTop(range: IRange, scrollType?: ScrollType): void;
        /**
         * Scroll vertically or horizontally as necessary and reveal a range close to the top of the viewport,
         * optimized for viewing a code definition. Only if it lies outside the viewport.
         */
        //revealRangeNearTopIfOutsideViewport(range: IRange, scrollType?: ScrollType): void;
        /**
         * Directly trigger a handler or an editor action.
         * @param source The source of the call.
         * @param handlerId The id of the handler or the id of a contribution.
         * @param payload Extra data to be sent to the handler.
         */
        public Task Trigger(string source, string handlerId, object payload = null)
        {
            var payloadJsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(payload));
            return JsRuntime.SafeInvokeAsync("blazorMonaco.editor.trigger", Id, source, handlerId, payloadJsonElement);
        }

        public Task RemoveContextMenuItems()
        {
            return JsRuntime.SafeInvokeAsync("blazorMonaco.editor.removeContextMenuItems", Id, new string[] { "vs.editor.ICodeEditor:1:setLabel" });
        }

        /**
         * Gets the current model attached to this editor.
         */
        //getModel(): IEditorModel | null;
        /**
         * Sets the current model attached to this editor.
         * If the previous model was created by the editor via the value key in the options
         * literal object, it will be destroyed. Otherwise, if the previous model was set
         * via setModel, or the model key in the options literal object, the previous model
         * will not be destroyed.
         * It is safe to call setModel(null) to simply detach the current model from the editor.
         */
        //setModel(model: IEditorModel | null): void;
        /**
         * Create a collection of decorations. All decorations added through this collection
         * will get the ownerId of the editor (meaning they will not show up in other editors).
         * These decorations will be automatically cleared when the editor's model changes.
         */
        //createDecorationsCollection(decorations?: IModelDeltaDecoration[]) : IEditorDecorationsCollection;
    }
}
