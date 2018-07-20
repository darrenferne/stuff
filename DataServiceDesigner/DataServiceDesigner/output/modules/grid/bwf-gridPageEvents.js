define(["require", "exports", "knockout", "loglevel", "sprintf", "modules/grid/bwf-clipboard", "modules/bwf-utilities"], function (require, exports, ko, log, sprintfM, clipboard, utils) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    var GridPageEvents = /** @class */ (function () {
        function GridPageEvents() {
            var _this = this;
            this.subscriptions = [];
            this.activeGridClass = 'active-bwf-grid';
            this.eventsAdded = false;
            this.lastPasteTime = 0;
            this.throttleMilliseconds = 100;
            var self = this;
            this.subscriptions.push(ko.postbox.subscribe("bwf-grid-addPageEvents", function (thisElement) {
                // add this each time that a grid is added to the page
                // e.g. opening a new view from a view
                // or opening a panel with a view in
                if ($(sprintf('.%s', _this.activeGridClass)).length === 0)
                    $(sprintf('#' + thisElement)).addClass(_this.activeGridClass);
                if (!self.eventsAdded) {
                    $(document)
                        .on('keydown', function (event) { return self.handleDocumentKeydown(event); })
                        .on('keyup', function (event) { return self.handleDocumentKeyup(event); });
                    // paste event listener - this only works because we focus 
                    // an input when CTRL is pressed (in IE anyway)
                    window.addEventListener('paste', function (ev) { return self.handlePasteEvent(self.getActiveGridViewModel(), ev); });
                    log.debug("Grid page events added");
                    self.eventsAdded = true;
                }
            }));
            this.subscriptions.push(ko.postbox.subscribe("set-active-grid", function (element) { return _this.setActiveGrid(element); }));
        }
        GridPageEvents.prototype.dispose = function () {
            this.subscriptions.forEach(function (x) { return x.dispose(); });
        };
        GridPageEvents.prototype.handlePasteEvent = function (grid, event) {
            if (!grid) {
                log.warn("Context for current grid could not be found");
                return;
            }
            // if our active element is not in a grid container we don't want to override the paste
            if (!$.contains(document.getElementById(grid.element), document.activeElement))
                return true;
            if (grid.config && !grid.config.inEditMode())
                return;
            if (grid.flags && grid.flags.applyInProgress()) {
                log.warn("Still applying previous updates");
                return;
            }
            var currentTime = window.performance.now();
            if (this.lastPasteTime + this.throttleMilliseconds < currentTime) {
                this.lastPasteTime = currentTime;
            }
            else {
                log.info(sprintf("Paste throttled - delay = %dms, lastPasteTime = %s, currentTime = %s", this.throttleMilliseconds, this.lastPasteTime, currentTime));
                return;
            }
            var clipboardData;
            var windowClipboardData = window.clipboardData;
            if (windowClipboardData !== undefined) {
                clipboardData = windowClipboardData.getData("Text");
            }
            else {
                clipboardData = event.clipboardData.getData('text/plain');
            }
            var self = this;
            var selected = grid.userSelectedCells();
            var isChoiceCell = selected && selected.length === 1 && selected[0].column.metadata.hasChoice;
            if (clipboard.getPasteDataType(clipboardData) === clipboard.PasteDataType.String && !isChoiceCell) {
                // let the normal event deal with the paste event
            }
            else {
                event.preventDefault();
                clipboard.processPastedData(grid, clipboardData);
            }
        };
        GridPageEvents.prototype.handleDocumentKeyup = function (event) {
            var grid = this.getActiveGridViewModel();
            if (!grid)
                return;
            // if our focused element is the copy paste textarea, we want
            // to return focus to the original element in the grid
            if (!event.target.classList.contains(grid.copyPasteTextAreaClass))
                return;
            switch (event.keyCode) {
                case utils.KEY_CODES.CTRL:
                    if (grid.activeElement()) {
                        $(grid.activeElement()).focus();
                        grid.activeElement(null);
                    }
                    break;
            }
        };
        GridPageEvents.prototype.isThereAnythingSelectedInGrid = function (grid) {
            if (grid.canSelectIndividualCells())
                return !!grid.originSelectedCell();
            else
                return grid.selectedRows().length > 0;
        };
        GridPageEvents.prototype.focusCopyPasteTextArea = function (grid) {
            $("." + grid.activeGridClass + " ." + grid.copyPasteTextAreaClass).focus().select();
        };
        GridPageEvents.prototype.handleDocumentKeydown = function (event) {
            var _this = this;
            var grid = this.getActiveGridViewModel();
            if (!grid)
                return true;
            var key = utils.KEY_CODES;
            if (!grid.isActiveGrid() || !this.isThereAnythingSelectedInGrid(grid))
                return;
            var targetElement = event.target;
            var activeElement = document.activeElement;
            if (activeElement) {
                var e = activeElement;
                var isPanel = false;
                var isGrid = false;
                while (e.parentElement != null && !(isPanel || isGrid)) {
                    isPanel = e.parentElement.classList.contains("panel-edit-input");
                    isGrid = e.parentElement.classList.contains("bwf-grid");
                    e = e.parentElement;
                }
                if (isPanel)
                    return;
                grid.activeElement(activeElement);
            }
            if (event.keyCode == key.CTRL) {
                if (targetElement.tagName.toLowerCase() === 'input') {
                    // just return, we'll deal with the copy or paste fine
                    return true;
                }
                else if (targetElement.tagName.toLowerCase() === 'textarea' &&
                    !targetElement.classList.contains(grid.copyPasteTextAreaClass)) {
                    // we haven't got any text areas in the grid so I've left this 
                    // unimplemented for now as there are quirks around it in IE!
                    // see http://stackoverflow.com/q/263743/1505511
                    log.warn("textarea copy/paste in the grid is currently unimplemented");
                }
                if (!utils.isAnySelectionOnPage()) {
                    // there will be an element in the grid with this class, so we
                    // focus it and select all the text in it, since IE can only copy
                    // text in a focused input that is selected.
                    this.focusCopyPasteTextArea(grid);
                }
            }
            var selectedCell = grid.originSelectedCell(), column = -1, row = -1, valueString = "";
            if (selectedCell) {
                var typeString = selectedCell.column.metadata.type.toLowerCase();
                if (typeString === 'measure')
                    valueString = selectedCell.value() ? selectedCell.value()["Value"] + selectedCell.value()["Unit"] : '';
                else if (typeString === 'date' || typeString === 'time')
                    valueString = selectedCell.column.formatter(selectedCell.value());
                else
                    valueString = selectedCell.value() ? selectedCell.value().toString() : '';
                column = selectedCell.column.position();
                row = selectedCell.row.position();
            }
            var target = event.target;
            var startOfInput = target.type !== "text"
                || target.selectionStart === 0 && target.selectionEnd === 0;
            var endOfInput = target.type !== "text"
                || target.selectionStart >= valueString.length;
            var anyTextSelected = target.type === "text" && target.selectionStart !== target.selectionEnd;
            var onLastRecord = row === grid.rows().length;
            var onLastCell = column === grid.columns().length;
            // if our active element is not in a grid container we don't want to override keys
            // we also have to check to see the active element isn't the body tag because IE
            // doesn't recognise our focused elements if they aren't text inputs
            if (document.activeElement.tagName.toLowerCase() !== 'body' &&
                !$.contains(document.getElementById(grid.element), document.activeElement))
                return true;
            // generic keys for grid
            switch (event.keyCode) {
                case key.C:
                    if (event.ctrlKey && grid.inEditMode() && !utils.isAnySelectionOnPage()) {
                        // focus copy paste input
                        // this works because the copy happens after the C event has been recognised
                        // this also means we must not prevent default or bubbling
                        this.focusCopyPasteTextArea(grid);
                    }
                    return true;
                case key.BACKSPACE:
                    if (this.shouldCancelBackspaceEvent(grid, targetElement))
                        event.preventDefault();
                    break;
                case key.Z:
                    // ctrl+z
                    if (event.ctrlKey && grid.inEditMode() && selectedCell) {
                        event.preventDefault();
                        this.updateCellValue(grid.viewGridId, selectedCell, selectedCell.originalValue);
                    }
                    return true;
                case key.ESCAPE:
                    if (selectedCell && selectedCell.value() !== selectedCell.originalValue)
                        this.updateCellValue(grid.viewGridId, selectedCell, selectedCell.originalValue);
                    else {
                        grid.clearSelected();
                        utils.removeAllSelectedRanges();
                    }
                    return;
                case key.A:
                    if (event.ctrlKey) {
                        // null this so that focus isn't reset to whatever cell
                        // we have initially selected when selecting all cells,
                        // since we want the 'origin cell'  to be cell at 1,1
                        grid.activeElement(null);
                        event.preventDefault();
                        ko.postbox.publish(grid.viewGridId + '-select-all');
                        if (!grid.canSelectIndividualCells())
                            grid.selectionChanged();
                        // refocus the copy paste area so when someone doesn't release the
                        // ctrl key they can still press Ctrl+C or Ctrl+X 
                        this.focusCopyPasteTextArea(grid);
                    }
                    return true;
                case key.X:
                    if (grid.inEditMode() && event.ctrlKey && (!anyTextSelected)) {
                        this.focusCopyPasteTextArea(grid);
                        setTimeout(function () {
                            // do NOT prevent default
                            if (grid.canSelectIndividualCells()) {
                                // clear the selected cells
                                var rows = [];
                                grid.userSelectedCells().forEach(function (cell) {
                                    if (cell.column.metadata.hasChoice || (!cell.column.metadata.isNotEditableInGrid && cell.value.isReadonly())) {
                                        _this.updateCellValue(grid.viewGridId, cell, "");
                                        if (rows.indexOf(cell.row) === -1)
                                            rows.push(cell.row);
                                    }
                                });
                                if (grid.updateDirtyRecordWithLatestValues)
                                    rows.forEach(function (row) { return grid.updateDirtyRecordWithLatestValues(row, grid.columns()); });
                            }
                            else {
                                grid.removeRecords();
                            }
                        });
                    }
                    // return true to do the original cut from the input
                    // this will also have the effect of "copying" from
                    // the grid when in display mode
                    return true;
                case key.ENTER:
                    if (grid.inEditMode()) {
                        utils.removeAllSelectedRanges();
                        event.preventDefault();
                        if (onLastRecord)
                            grid.insertRecord(false);
                        grid.selectCellAt(column, row + 1);
                    }
                    return;
                case key.TAB:
                    if (grid.inEditMode()) {
                        utils.removeAllSelectedRanges();
                        event.preventDefault();
                        if (onLastCell && onLastRecord && !event.shiftKey)
                            grid.insertRecordAtBottom();
                        var coords = event.shiftKey
                            ? grid.previousCellFrom(column, row)
                            : grid.nextCellFrom(column, row);
                        grid.selectCellAt(coords.Column, coords.Row);
                    }
                    return;
                case key.I:
                case key.R:
                    if (grid.inEditMode() && event.altKey) {
                        event.preventDefault();
                        if (event.keyCode == key.I)
                            grid.insertRecord();
                        if (event.keyCode == key.R)
                            grid.removeRecords();
                    }
                    return true;
            }
            // selection changed keys
            if (grid.canSelectIndividualCells()) {
                switch (event.keyCode) {
                    case key.ARROW_LEFT:
                        if (selectedCell && !(event.ctrlKey || event.shiftKey)) {
                            if (selectedCell.column.metadata.isNotEditableInGrid || startOfInput || selectedCell.value.isReadonly()) {
                                var previous = grid.previousCellFrom(column, row);
                                // don't change selection if we are going to select the same cell
                                if (previous.Row === selectedCell.row.position() && previous.Column === selectedCell.column.position())
                                    return;
                                utils.removeAllSelectedRanges();
                                event.preventDefault();
                                grid.selectCellAt(previous.Column, previous.Row);
                                grid.selectionChanged();
                            }
                        }
                        break;
                    case key.ARROW_UP:
                        if (!(event.ctrlKey || event.shiftKey)) {
                            utils.removeAllSelectedRanges();
                            event.preventDefault();
                            grid.selectCellAt(column, row - 1);
                            grid.selectionChanged();
                        }
                        break;
                    case key.ARROW_RIGHT:
                        if (selectedCell && !(event.ctrlKey || event.shiftKey)) {
                            if (selectedCell.column.metadata.isNotEditableInGrid || endOfInput || selectedCell.value.isReadonly()) {
                                var next = grid.nextCellFrom(column, row);
                                // don't change selection if we are going to select the same cell
                                if (next.Row === selectedCell.row.position() && next.Column === selectedCell.column.position())
                                    return;
                                utils.removeAllSelectedRanges();
                                event.preventDefault();
                                grid.selectCellAt(next.Column, next.Row);
                                grid.selectionChanged();
                            }
                        }
                        break;
                    case key.ARROW_DOWN:
                        if (!(event.ctrlKey || event.shiftKey)) {
                            utils.removeAllSelectedRanges();
                            event.preventDefault();
                            grid.selectCellAt(column, row + 1);
                            grid.selectionChanged();
                        }
                        break;
                    default:
                        break;
                }
            }
            else {
                switch (event.keyCode) {
                    case key.ARROW_UP:
                        event.preventDefault();
                        event.stopPropagation();
                        var firstSelectedRowPos = grid.selectedRows()[0].position();
                        if (firstSelectedRowPos > 1) {
                            if (!event.shiftKey)
                                grid.clearSelected();
                            grid.rows()[firstSelectedRowPos - 2].selected(true);
                            grid.selectionChanged();
                        }
                        break;
                    case key.ARROW_DOWN:
                        event.preventDefault();
                        event.stopPropagation();
                        var lastSelectedRowElement = grid.selectedRows().length - 1;
                        var lastSelectedRowPos = grid.selectedRows()[lastSelectedRowElement].position();
                        if (lastSelectedRowPos < grid.rows().length) {
                            if (!event.shiftKey)
                                grid.clearSelected();
                            grid.rows()[lastSelectedRowPos].selected(true);
                            grid.selectionChanged();
                        }
                        break;
                }
            }
        };
        GridPageEvents.prototype.updateCellValue = function (viewGridId, cell, value) {
            var publishString = viewGridId + "-update-cell-" + cell.row.position() + "-" + cell.column.position();
            var formattedValue = cell.column.formatter(value);
            ko.postbox.publish(publishString, formattedValue || value);
        };
        GridPageEvents.prototype.shouldCancelBackspaceEvent = function (grid, target) {
            if ($.contains(document.getElementById(grid.element), target)) {
                switch (target.tagName.toLowerCase()) {
                    case 'input':
                        var inputElement = target;
                        // if it's readonly we are going to cancel
                        if (inputElement.readOnly)
                            return true;
                        switch (inputElement.type.toLowerCase()) {
                            // the allowed ones
                            case 'text':
                            case 'password':
                            case 'date':
                            case 'time':
                            case 'datetime':
                            case 'datetime-local':
                            case 'email':
                            case 'url':
                                return false;
                            // for the rest, we are going to cancel the event
                            // this includes radio/checkbox, buttons
                            default:
                                return true;
                        }
                    case 'textarea':
                        var textAreaElement = target;
                        // if it's readonly we are going to cancel
                        // if readonly isn't defined it isn't readonly
                        return textAreaElement.readOnly || false;
                    default:
                        // includes select and if we are focused on
                        // the grid but not a specific cell, for
                        // example if we have a row selected we don't
                        // know what the event target will be
                        return true;
                }
            }
            return true;
        };
        GridPageEvents.prototype.setActiveGrid = function (element) {
            // remove currently active classes 
            $(sprintf('.%s', this.activeGridClass)).removeClass(this.activeGridClass);
            var bwfGridClass = 'bwf-grid-container';
            // set active
            var $target = $(sprintf('#%s', element));
            if ($target.hasClass(bwfGridClass)) {
                $target.addClass(this.activeGridClass);
                log.debug("Active grid set to clicked grid");
            }
            else {
                var closest = $target.closest('.' + bwfGridClass);
                if (closest.length === 1) {
                    $(closest[0]).addClass(this.activeGridClass);
                    log.debug("Active grid set to closest bwf-grid to click");
                }
                else if (closest.length == 0) {
                    var findResult = $target.find('.' + bwfGridClass);
                    if (findResult.length === 1) {
                        $(findResult[0]).addClass(this.activeGridClass);
                        log.debug("Active grid set to descendant of clicked target");
                    }
                }
            }
            return true;
        };
        GridPageEvents.prototype.getActiveGridViewModel = function () {
            var activeGrid = $(sprintf('.%s', this.activeGridClass))[0];
            if (!activeGrid)
                return null;
            var gridKoModel = ko.contextFor($(activeGrid).closest('.bwf-grid-container')[0])['$component'];
            return gridKoModel;
        };
        return GridPageEvents;
    }());
    return new GridPageEvents();
});
