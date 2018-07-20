define(["require", "exports", "knockout", "options", "loglevel", "sprintf", "modules/bwf-propertyReader", "modules/bwf-bindingHandlers", "modules/bwf-utilities", "modules/grid/bwf-clipboard", "modules/grid/bwf-gridPageEvents"], function (require, exports, ko, options, log, sprintfM, pathReader, bindingHandlers, utils, clipboard, gridPageEvents) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    // force typescript to require
    var bh = bindingHandlers;
    var gpe = gridPageEvents;
    var sprintf = sprintfM.sprintf;
    var bottomPosition = 1000;
    var GridViewModel = /** @class */ (function () {
        function GridViewModel(configuration) {
            var _this = this;
            this.aggregateRowsSelected = ko.observableArray([]);
            this.maxRows = 150;
            this.orderedBy = null;
            this.r = options.resources;
            this.subscriptions = [];
            this.viewId = ko.observable(0);
            this.viewGridId = '';
            this.viewName = ko.observable('');
            this.postRenderCallback = null;
            this.usingCombinedFooter = ko.observable(false);
            this.usingCombinedHeader = ko.observable(false);
            /* for copy/paste */
            this.activeElement = ko.observable(null);
            this.columnsCount = function () { return _this.columns().length; };
            this.rowCount = function () { return _this.rows().length; };
            this.activeGridClass = 'active-bwf-grid';
            this.copyPasteTextAreaClass = 'copy-paste-focusable';
            this.notInEditMode = ko.pureComputed(function () { return !_this.inEditMode(); });
            this.rowComponent = function (record) { return record.getRowComponent(_this); };
            this.showAggregatesAtTop = ko.pureComputed(function () { return _this.config.aggregatesPosition && _this.config.aggregatesPosition() === 'top' && _this.aggregates().length > 0; });
            this.showAggregatesAtBottom = ko.pureComputed(function () { return _this.config.aggregatesPosition && _this.config.aggregatesPosition() === 'bottom' && _this.aggregates().length > 0; });
            this.originSelectedCell = ko.observable(null);
            this.validate = function (item, success, failure) {
                failure({
                    ModelValidations: [],
                    PropertyValidations: {},
                    Summary: 'Validation not configured'
                });
            };
            this.selectionThrottle = 10;
            this.selectAllRows = function () {
                _this.rows()
                    .filter(function (r) { return !r._destroy; })
                    .forEach(function (r) { return r.selected(true); });
                ko.postbox.publish(_this.viewGridId + '-selection-changed');
            };
            this.selectAllCells = function () {
                _this.clearSelected();
                _this.rows().forEach(function (r, recordIndex) { return Object.keys(r.values).forEach(function (key, colIndex) {
                    if (recordIndex === 0 && colIndex == 0)
                        _this.selectCellAt(colIndex + 1, r.position());
                    else
                        r.values[key].isInCopyOrPasteGroup(true);
                }); });
                ko.postbox.publish(_this.viewGridId + '-selection-changed');
            };
            this.validationOnSuccess = function (row, record) {
                row.resetValidation();
                Object.keys(row.values)
                    .forEach(function (key) {
                    if (row.values[key].isSelected() &&
                        (_this.originSelectedCell() && !_this.originSelectedCell().column.metadata.isNotEditableInGrid)) {
                        // don't update the currently selected cell (unless it's not editable), since by the time the 
                        // validation callback is run the user may have edited it, and 
                        // wiping out their changes would be really rude
                        return;
                    }
                    row.values[key](pathReader.getPropertyValue(key, record));
                    row.values[key].isValid(true);
                });
                row.dirtyRecord = record;
            };
            this.aggregates = configuration.aggregates || ko.observableArray([]);
            this.config = configuration;
            this.flags = configuration.flags;
            this.footer = configuration.footer;
            this.header = configuration.header;
            this.inEditMode = configuration.inEditMode || ko.observable(false);
            this.isView = configuration.isView;
            this.orderedBy = configuration.orderedBy;
            this.showValidationInDisplayMode = configuration.showValidationInDisplayMode || false;
            this.viewGridId = configuration.viewGridId;
            this.element = configuration.viewGridId + '-grid-container';
            this.disabled = configuration.disabled || ko.observable(false);
            this.disableGridSorting = configuration.disableGridSorting;
            this.disableSoftDelete = !!configuration.disableSoftDelete && true;
            this.disableTextSelectionInGrid = ko.observable(true);
            this.postRenderCallback = configuration.postRender;
            this.embedded = configuration.embedded || false;
            this.recordsCount = configuration.recordsCount || ko.pureComputed(function () { return configuration.records().length; });
            this.canInsertRows = ko.pureComputed(function () {
                if (configuration.metadata() == null)
                    return false;
                var insertionDisabled = configuration.disableInsertRecords;
                var canInsert = configuration.metadata().insertableInEditMode;
                var canCreate = configuration.createNewRecord != null;
                var tooManyRecords = _this.recordsCount() >= _this.maxRows;
                return !insertionDisabled && !tooManyRecords && canInsert && canCreate;
            });
            this.updateDirtyRecordWithLatestValues = configuration.updateDirtyRecordWithLatestValues;
            this.canRemoveRows = ko.pureComputed(function () {
                if (configuration.metadata() == null)
                    return false;
                else
                    return !configuration.disableRemoveRecords && configuration.metadata().deletableInEditMode;
            });
            this.headerVisible = ko.pureComputed(function () { return (!_this.usingCombinedHeader() && _this.header.enabled()); });
            this.footerVisible = ko.pureComputed(function () { return !_this.usingCombinedFooter() && _this.footer.enabled(); });
            this.canSelectIndividualCells = configuration.canSelectIndividualCells || ko.observable(false);
            this.subscriptions.push(this.canSelectIndividualCells.subscribe(function (cellSelect) {
                if (cellSelect) {
                    var selectedRows = _this.selectedRows();
                    var rowPositions = selectedRows.map(function (x) { return x.position(); });
                    var uniqueRowPositions = rowPositions.filter(function (v, index, self) { return self.indexOf(v) == index; });
                    // select all cells in selected row areas
                    _this.clearSelected(function () {
                        uniqueRowPositions.forEach(function (rowNumber) {
                            return _this.columns().forEach(function (column) {
                                return _this.selectCellAt(column.position(), rowNumber, true);
                            });
                        });
                        _this.selectionChanged();
                    });
                }
                else {
                    var rows = _this.userSelectedCells().map(function (x) { return x.row.position(); });
                    var uniqueRows = rows.filter(function (r, index, self) { return self.indexOf(r) == index; });
                    // select all rows where there are selected cells
                    _this.clearSelected(function () {
                        _this.rows().forEach(function (x) { return uniqueRows.some(function (u) { return x.position() === u; }) ? x.selected(true) : x.selected(false); });
                        _this.selectionChanged();
                    });
                }
            }));
            if (configuration.isView) {
                this.viewId(configuration.view.Id);
                this.viewName(configuration.view.Name);
            }
            if (configuration.validate)
                this.validate = configuration.validate;
            this.columns = ko.pureComputed(function () { return _this.config.selectedColumns().sort(function (left, right) { return left.position() - right.position(); }); });
            this.rows = ko.computed(function () {
                var rows = _this.config.records()
                    .filter(function (r) { return !r['_destroy']; })
                    .sort(function (left, right) { return left.queryPosition() - right.queryPosition(); })
                    .map(function (record, index) { return (record.position(index + 1), record); });
                return rows.sort(function (l, r) { return l.position() - r.position(); });
            });
            // need the rows computed to update immediatly for cases where a grid has auto-updates
            // disabled and we fetch the latest values prior to entering edit mode. If defer is not
            // set to false then we still end up editing the old values as the templates render with a 
            // stale value
            this.rows['deferUpdates'] = false;
            this.selectedRows = ko.pureComputed(function () { return _this.rows().filter(function (x) { return x.selected(); }); });
            this.userSelectedCells = ko.pureComputed(function () {
                var cells = [];
                _this.rows().forEach(function (row) {
                    var keys = Object.keys(row.values);
                    keys.forEach(function (key) {
                        var thisCol = _this.columns().filter(function (x) { return x.path == key; })[0];
                        var value = row.values[key];
                        var originalValue = null;
                        // thisCol might be null when we leave edit mode when there
                        // were mandatory columns added, as the column that was
                        // added has now been removed and the value is still there
                        //
                        // we don't need the original value in display mode
                        // anyway so we are fine to set this to null
                        if (thisCol)
                            originalValue = pathReader.getPropertyValue(thisCol.path, row.dirtyRecord || row.record);
                        var newSelectedCell = {
                            column: thisCol,
                            row: row,
                            value: value,
                            originalValue: originalValue
                        };
                        if (value.isInCopyOrPasteGroup() || value.isSelected())
                            cells.push(newSelectedCell);
                    });
                });
                return cells;
            });
            if (document.getElementsByClassName(this.activeGridClass).length === 0)
                $(document.getElementById(this.element)).addClass(this.activeGridClass);
            this.subscriptions.push(this.inEditMode.subscribe(function (inEditMode) {
                _this.clearSelected();
                if (inEditMode) {
                    _this.disableTextSelectionInGrid(false);
                    _this.canSelectIndividualCells(true);
                    if (_this.rowCount() > 0)
                        _this.selectCellAt(1, 1);
                }
                else {
                    _this.disableTextSelectionInGrid(true);
                    _this.canSelectIndividualCells(false);
                }
            }));
            this.subscriptions.push(ko.postbox.subscribe(this.viewGridId + '-delete-row', function (cb) { return _this.removeRecords(cb); }));
            this.subscriptions.push(ko.postbox.subscribe(this.viewGridId + '-insert-row', function (options) {
                var insertAbove = true;
                if (typeof options === 'boolean')
                    insertAbove = options;
                else if (options != null && typeof options === 'object' && typeof options.insertAbove === 'boolean')
                    insertAbove = options.insertAbove;
                var newRecord = _this.insertRecord(insertAbove);
                if (newRecord == null)
                    return;
                var selected = _this.originSelectedCell();
                if (_this.canSelectIndividualCells())
                    _this.selectCell(selected ? selected.column : _this.columns()[0], newRecord);
                else
                    _this.clearSelected(function () { return newRecord.selected(true); });
                if (options != null && typeof options === 'object' && typeof options.callback === 'function')
                    options.callback(newRecord);
            }));
            this.subscriptions.push(ko.postbox.subscribe(this.viewGridId + '-selection-changed', function () { return _this.selectionChanged(); }));
            this.subscriptions.push(ko.postbox.subscribe(this.viewGridId + '-update-clipboard-value', function () { return _this.copyToClipboardInput(); }));
            this.subscriptions.push(ko.postbox.subscribe(this.viewGridId + '-selection-cleared', function () { return _this.clearCopyPasteBox(); }));
            this.subscriptions.push(ko.postbox.subscribe(this.viewGridId + '-select-all', function () {
                if (_this.canSelectIndividualCells())
                    _this.selectAllCells();
                else
                    _this.selectAllRows();
            }));
            this.subscriptions.push(ko.postbox.subscribe(this.viewGridId + '-validate-row', function (bwfId) {
                var rows = _this.rows().filter(function (x) { return x.bwfId === bwfId; });
                if (rows.length === 1)
                    _this.validate(rows[0], _this.validationOnSuccess, function (validatedRow, validation) { return validatedRow.applyValidation(validation); });
            }));
            log.debug(sprintf("Grid '%s' initialised", this.viewGridId));
        }
        GridViewModel.prototype.dispose = function () {
            ko.postbox.publish('bwf-grid-disposing', this.element);
            this.subscriptions.forEach(function (s) { return s.dispose(); });
            this.notInEditMode.dispose();
            this.columns.dispose();
            this.rows.dispose();
            this.showAggregatesAtBottom.dispose();
            this.showAggregatesAtTop.dispose();
            var table = $(document.getElementById(this.element)).find(' table');
            var tableWrapper = $(document.getElementById(this.element))
                .find('.bwf-grid-wrapper');
            table.off('click', 'th');
            table.off('click', 'td');
            table.off('keydown', 'td');
            tableWrapper.off('mousedown', '*');
            tableWrapper.off('mousemove', '*');
            tableWrapper.off('mouseup');
            log.debug("Disposed of grid " + this.element);
        };
        // functions
        GridViewModel.prototype.postRender = function () {
            this.addEventHooks();
            if (typeof this.postRenderCallback === "function")
                this.postRenderCallback();
        };
        GridViewModel.prototype.addEventHooks = function () {
            var _this = this;
            this.subscriptions.push(ko.postbox.subscribe(this.element + "-use-combined-footer", function () {
                _this.usingCombinedFooter(true);
            }));
            this.subscriptions.push(ko.postbox.subscribe(this.element + "-use-combined-header", function () {
                _this.usingCombinedHeader(true);
                if (_this.header.config != null)
                    _this.header.config.showTitle(false);
            }));
            var self = this;
            var table = $(document.getElementById(this.element)).find('table');
            $(document.getElementById(this.element)).on('click', '.bwf-grid-wrapper', function (event) {
                ko.postbox.publish('set-active-grid', self.element);
            });
            table.on('mouseup', 'td:not(.bwf-grid-no-event)', function (event) {
                if (event.target.classList.contains('spacer-cell') && self.canSelectIndividualCells()) {
                    self.clearSelected();
                    utils.removeAllSelectedRanges();
                }
                else {
                    var context = ko.contextFor(this);
                    self.cellClicked(context, event.shiftKey, event.ctrlKey);
                    self.selectionChanged();
                }
            });
            // disable html5 drag event
            // this stops the text to be being dragged,
            // which avoids issues when trying to box select
            table.on('dragstart', function () { return false; });
            // if we override the focus event for the checkbox, 
            // toggling with the spacebar doesn't work in IE
            table.on('focus', 'input:not([type="checkbox"])', function (event) {
                var element = event.target;
                var type = element.type.toLowerCase();
                // now we select the whole contents of the cell
                // unless the input (or the cell itself) is marked as not-editable
                var typeIsText = type === 'text';
                var targetIsEditable = !element.classList.contains('not-editable');
                var targetParentIsEditable = !element.parentElement.classList.contains('not-editable');
                if (typeIsText && targetIsEditable && targetParentIsEditable) {
                    // if we are selecting multiple cells, we prevent the selection
                    // change as we can keep the original cell contents selected
                    if (event.shiftKey || event.ctrlKey) {
                        event.preventDefault();
                        return;
                    }
                    if (element.value == '')
                        return;
                    // select all contents of cell input
                    // we can't use 'one' because that binds seperate handlers to 
                    // both event types, whereas we want one handler for both
                    // see http://stackoverflow.com/a/24589806/1505511
                    $(this).on("click keyup", function (e) {
                        $(this).off("click keyup").select();
                    });
                }
                else {
                    // deselect everything if we are selecting multiple cells
                    utils.removeAllSelectedRanges();
                }
                return;
            });
            if (this.orderedBy) {
                table.on('click', 'th', function (event) {
                    var context = ko.contextFor(this);
                    var model = ko.dataFor(this);
                    self.headingClicked(context, model);
                });
            }
            ko.postbox.publish("bwf-grid-addPageEvents", this.element);
            this.addClickDragEventHooks();
        };
        GridViewModel.prototype.addClickDragEventHooks = function () {
            var table = $(document.getElementById(this.element)).find(' table');
            var tableWrapper = $(document.getElementById(this.element))
                .find('.bwf-grid-wrapper');
            var self = this;
            var dragging = false;
            var drawing = false;
            var startedAt = 0;
            var startx = 0;
            var starty = 0;
            var startTableCell = null;
            var currentx = 0;
            var currenty = 0;
            var box = null;
            var callbackId = 0;
            var callback = function () {
                box.style.left = (startx < currentx ? startx : currentx) + "px";
                box.style.top = (starty < currenty ? starty : currenty) + "px";
                box.style.width = Math.abs(startx - currentx).toString() + "px";
                box.style.height = Math.abs(starty - currenty).toString() + "px";
                callbackId = window.requestAnimationFrame(callback);
            };
            var onMouseDown = function (event) {
                var $target = $(event.target);
                if ($target.hasClass('display-cancel-box-select') ||
                    $target.hasClass("bwf-grid-no-event") ||
                    $target.is("select:enabled"))
                    return;
                if (event.shiftKey || event.ctrlKey) {
                    event.preventDefault();
                    return false;
                }
                dragging = true;
                startedAt = window.performance.now();
                startx = event.clientX;
                starty = event.clientY;
                var startMoveTdElements = utils.elementsFromPoint(startx, starty)
                    .filter(function (x) { return x.tagName.toLowerCase() === 'td'; });
                if (startMoveTdElements.length > 0)
                    startTableCell = startMoveTdElements[0];
            };
            var onMouseMove = function (event) {
                if (!dragging)
                    return;
                currentx = event.clientX;
                currenty = event.clientY;
                var currentMoveTdElements = utils.elementsFromPoint(currentx, currenty)
                    .filter(function (x) { return x.tagName.toLowerCase() === 'td'; });
                var currentTableCell;
                if (currentMoveTdElements.length > 0)
                    currentTableCell = currentMoveTdElements[0];
                // don't start the drag until we have left the start element
                if (currentTableCell === startTableCell)
                    return;
                // also don't start drawing the rectangle immediately, add a little delay so
                // we can be certain a click & drag is intentional
                if (!drawing && window.performance.now() - startedAt < 100)
                    return;
                event.preventDefault();
                self.disableTextSelectionInGrid(true);
                if (!drawing) {
                    drawing = true;
                    box = document.createElement("div");
                    box.id = self.element + '-selection-box';
                    box.classList.add('bwf-selection-box');
                    document.childNodes[1].appendChild(box);
                    $(box).on('mousemove', onMouseMove); // for IE
                    callbackId = window.requestAnimationFrame(callback);
                }
            };
            var onMouseUp = function (event) {
                dragging = false;
                drawing = false;
                startTableCell = null;
                var endx = event.clientX;
                var endy = event.clientY;
                self.disableTextSelectionInGrid(false);
                window.cancelAnimationFrame(callbackId);
                if (box) {
                    var elementsAtStart = utils.elementsFromPoint(startx, starty);
                    var elementsAtEnd = utils.elementsFromPoint(endx, endy);
                    var startCell = $(elementsAtStart).filter(function (x, el) { return el.tagName.toLowerCase() == 'td'; })[0];
                    var endCell = $(elementsAtEnd).filter(function (x, el) { return el.tagName.toLowerCase() == 'td'; })[0];
                    $(document.getElementById(self.element + '-selection-box')).remove();
                    box = null;
                    event.preventDefault();
                    if (!startCell || !endCell)
                        return;
                    var start = 1;
                    var end = self.rowCount();
                    start = startCell.parentElement.rowIndex;
                    end = endCell.parentElement.rowIndex;
                    // if we have the ctrl key down we want to keep the current selection
                    if (!event.ctrlKey)
                        self.clearSelected();
                    if (!self.canSelectIndividualCells()) {
                        var recordsToSelect;
                        if (start < end)
                            recordsToSelect = self.rows().filter(function (r) { return r.position() >= start && r.position() <= end; });
                        else
                            recordsToSelect = self.rows().filter(function (r) { return r.position() >= end && r.position() <= start; });
                        recordsToSelect.forEach(function (r) { return r.selected(true); });
                    }
                    else {
                        var startRow = $(startCell.parentElement).children("td");
                        var endRow = $(endCell.parentElement).children("td");
                        var startIndex = startRow.index(startCell);
                        var endIndex = endRow.index(endCell);
                        var startCellColumn_1 = startIndex + 1;
                        var endCellColumn_1 = endIndex + 1;
                        //Add any colspans from previous cells in the row
                        startRow.slice(0, startIndex).toArray().forEach(function (cell) {
                            startCellColumn_1 += (cell.colSpan ? cell.colSpan : 1) - 1;
                        });
                        endRow.slice(0, endIndex).toArray().forEach(function (cell) {
                            endCellColumn_1 += (cell.colSpan ? cell.colSpan : 1) - 1;
                        });
                        if (self.inEditMode() || self.config.showValidationInDisplayMode) {
                            startCellColumn_1--;
                            endCellColumn_1--;
                        }
                        // we always need an origin cell when selecting
                        if (!event.ctrlKey || (event.ctrlKey && !self.originSelectedCell())) {
                            // select the "origin" cell
                            self.selectCellAt(startCellColumn_1, start);
                        }
                        self.selectCellsInRange(startCellColumn_1, start, endCellColumn_1, end);
                    }
                    self.selectionChanged();
                    // deselect all selected text
                    // this is so we don't get copy/clipboard focus issues
                    utils.removeAllSelectedRanges();
                    // set the caret to the end of the input for the start cell
                    var inputElementSelector = $(startCell).children("input[type='text']");
                    if (inputElementSelector[0]) {
                        var inputElement = inputElementSelector[0];
                        utils.setCaretPosition(inputElement, inputElement.value.length);
                    }
                }
            };
            tableWrapper.on('mousedown', onMouseDown);
            tableWrapper.on('mousemove', onMouseMove);
            $(document).on('mouseup', onMouseUp);
        };
        GridViewModel.prototype.clearCopyPasteBox = function () {
            $(sprintf(".%s .%s", this.activeGridClass, this.copyPasteTextAreaClass)).val("");
            log.debug("Cleared copy/paste input");
        };
        GridViewModel.prototype.selectionChanged = function () {
            ko.postbox.publish('set-active-grid', this.element);
            if (window.performance.now() + this.selectionThrottle < this.lastSelectionChangeTime) {
                log.debug("Selection change throttled");
                return;
            }
            this.copyToClipboardInput();
        };
        GridViewModel.prototype.copyToClipboardInput = function () {
            var startTime = window.performance.now();
            var clipboardData = clipboard.getCopiedData(this.canSelectIndividualCells(), this.rows(), this.columns());
            if (!clipboardData) {
                return;
            }
            this.lastSelectionChangeTime = window.performance.now();
            $(sprintf(".%s .%s", this.activeGridClass, this.copyPasteTextAreaClass)).val(clipboardData);
            var endTime = window.performance.now();
            log.debug(sprintf("Copy data generated - time taken %fms", endTime - startTime));
        };
        GridViewModel.prototype.isActiveGrid = function () {
            var el = document.getElementById(this.element);
            if (!el)
                return false;
            return el.classList.contains(this.activeGridClass);
        };
        ;
        GridViewModel.prototype.aggregateConfig = function () {
            var config = {
                aggregates: this.aggregates,
                aggregateRowsSelected: this.aggregateRowsSelected,
                columns: this.columns
            };
            return config;
        };
        GridViewModel.prototype.insertRecord = function (insertAbove) {
            if (insertAbove === void 0) { insertAbove = true; }
            if (!this.canInsertRows())
                return;
            var selected = this.originSelectedCell();
            var newRecord = this.config.createNewRecord();
            if (newRecord == null)
                return null;
            var selectedRows = this.selectedRows();
            var position = -1;
            if (selected) {
                position = selected.row.queryPosition() + (insertAbove ? -0.5 : 0.5);
            }
            else if (selectedRows.length > 0) {
                if (insertAbove)
                    position = selectedRows[0].queryPosition() - 0.5;
                else
                    position = selectedRows[selectedRows.length - 1].queryPosition() + 0.5;
            }
            else {
                position = insertAbove ? -1 : bottomPosition++;
            }
            newRecord.queryPosition(position);
            this.config.records.push(newRecord);
            // order the query positions
            this.config.records().sort(function (l, r) { return l.queryPosition() - r.queryPosition(); }).forEach(function (x, index) { return x.queryPosition(index); });
            return newRecord;
        };
        GridViewModel.prototype.insertRecordAtBottom = function () {
            if (!this.canInsertRows())
                return;
            var newRecord = this.config.createNewRecord();
            newRecord.queryPosition(bottomPosition++);
            this.config.records.push(newRecord);
            return newRecord;
        };
        GridViewModel.prototype.getRowToSelectAfterRemove = function () {
            var selectedCells = this.userSelectedCells();
            var nextSelectedRow;
            var currentHighestNumberedRecordPosition;
            var currentLowestNumberRecordPosition;
            if (this.canSelectIndividualCells()) {
                currentHighestNumberedRecordPosition = Math.max.apply(Math, selectedCells.map(function (x) { return x.row; }).map(function (o) { return o.position(); }));
                currentLowestNumberRecordPosition = Math.min.apply(Math, selectedCells.map(function (x) { return x.row; }).map(function (o) { return o.position(); }));
            }
            else {
                var selectedRows = this.selectedRows();
                currentHighestNumberedRecordPosition = Math.max.apply(Math, selectedRows.map(function (o) { return o.position(); }));
                currentLowestNumberRecordPosition = Math.min.apply(Math, selectedRows.map(function (o) { return o.position(); }));
            }
            var nextRowFilter = this.rows().filter(function (x) { return x.position() === currentHighestNumberedRecordPosition + 1; });
            if (nextRowFilter.length === 1) {
                nextSelectedRow = nextRowFilter[0];
            }
            else {
                nextRowFilter = this.rows().filter(function (x) { return x.position() === currentLowestNumberRecordPosition - 1; });
                if (currentLowestNumberRecordPosition > 0 && nextRowFilter.length === 1)
                    nextSelectedRow = nextRowFilter[0];
            }
            return nextSelectedRow;
        };
        GridViewModel.prototype.removeRecords = function (opts) {
            var _this = this;
            var originCell = this.originSelectedCell();
            var selectedCells = this.userSelectedCells();
            var recordsToRemove = [];
            var rowToSelectAfterRemove = this.getRowToSelectAfterRemove();
            if (this.canSelectIndividualCells() && selectedCells.length > 0) {
                // get records we need to remove
                selectedCells.forEach(function (cell) {
                    if (recordsToRemove.indexOf(cell.row) === -1)
                        recordsToRemove.push(cell.row);
                });
            }
            else if (this.selectedRows().length > 0) {
                recordsToRemove = this.selectedRows();
            }
            recordsToRemove.forEach(function (rec) {
                if (rec.isNewRecord() || _this.disableSoftDelete) {
                    var removed = _this.config.records.remove(function (r) { return r.bwfId === rec.bwfId; });
                    if (removed.length === 0)
                        _this.config.records.destroy(function (r) { return r.bwfId === rec.bwfId; });
                }
                else {
                    rec._destroy = true;
                    rec.updateType("Deleted");
                }
            });
            // the _destroy flag KO sets isnt an observable, so since processing the 
            // change set may have "undeleted" some objects we need to tell KO that the 
            // array has mutated so any undeleted records reappear in the grid
            this.config.records.valueHasMutated();
            // now select the next cell or row
            if (recordsToRemove.length > 0) {
                if (originCell && this.canSelectIndividualCells()) {
                    if (rowToSelectAfterRemove != null)
                        this.selectCell(originCell.column, rowToSelectAfterRemove);
                }
                else if (!this.canSelectIndividualCells()) {
                    var originalPosition = recordsToRemove[0].position();
                    if (rowToSelectAfterRemove != null)
                        rowToSelectAfterRemove.selected(true);
                }
            }
            if (opts && opts.callback && typeof opts.callback === "function")
                opts.callback();
        };
        GridViewModel.prototype.rowConfig = function (record) {
            var config = {
                columns: this.columns,
                row: record,
                typeMetadata: this.config.metadata(),
                validationInDisplayMode: this.showValidationInDisplayMode,
                gridId: this.viewGridId,
                gridDisabled: this.disabled,
                highlight: record.highlight
            };
            return config;
        };
        // event & click handlers
        GridViewModel.prototype.cellClicked = function (context, shiftKey, ctrlKey) {
            if (this.canSelectIndividualCells()) {
                if (shiftKey)
                    this.selectCellMultiple(context.column, context.record);
                else
                    this.selectCell(context.column, context.record, ctrlKey);
            }
            else {
                this.selectRow(context, shiftKey, ctrlKey);
            }
        };
        GridViewModel.prototype.selectRow = function (context, shiftKey, ctrlKey) {
            if (shiftKey) {
                this.selectMultipleRow(context, ctrlKey);
            }
            else {
                this.selectSingleRow(context, ctrlKey);
            }
        };
        GridViewModel.prototype.selectSingleRow = function (context, ctrlKey) {
            if (!ctrlKey) {
                this.aggregateRowsSelected([]);
                this.clearSelected();
            }
            if (context.record) {
                if (context.record.selected())
                    context.record.selected(false);
                else
                    context.record.selected(true);
            }
            else if (typeof context.$parent === 'number') {
                this.aggregateRowsSelected.push(context.$parent);
            }
            this.selectionChanged();
        };
        GridViewModel.prototype.selectMultipleRow = function (context, ctrlKey) {
            if (!context.record)
                return;
            var selectedRows = this.config.selectedRecords();
            if (selectedRows.length === 0)
                this.selectSingleRow(context, ctrlKey);
            var existingSelectedRow = selectedRows[0].position();
            var row = context.record;
            var recordsToSelect = [];
            if (row.position() > existingSelectedRow)
                recordsToSelect = this.rows().filter(function (r) { return r.position() <= row.position()
                    && r.position() >= existingSelectedRow; });
            else
                recordsToSelect = this.rows().filter(function (r) { return r.position() >= row.position()
                    && r.position() <= existingSelectedRow; });
            recordsToSelect.forEach(function (r) { return r.selected(true); });
            this.selectionChanged();
        };
        GridViewModel.prototype.nextCellFrom = function (columnIndex, rowIndex) {
            var rowCount = this.rowCount();
            var columnCount = this.columnsCount();
            if (columnIndex < columnCount) {
                var row = this.rows().filter(function (r) { return r.position() == rowIndex; })[0];
                var column = this.columns().filter(function (c) { return c.position() === columnIndex; })[0];
                var colSpan = 1;
                if (row && column) {
                    var item = row.values[column.path];
                    if (item)
                        colSpan = item.colSpan ? item.colSpan : 1;
                }
                return { Column: columnIndex + colSpan, Row: rowIndex };
            }
            if (rowIndex < rowCount)
                return { Column: 1, Row: rowIndex + 1 };
            return { Column: columnIndex, Row: rowIndex };
        };
        GridViewModel.prototype.previousCellFrom = function (columnIndex, rowIndex) {
            var _this = this;
            var prevRowIndex = 1;
            var prevColIndex = 1;
            if (columnIndex > 1) {
                prevRowIndex = rowIndex;
                prevColIndex = columnIndex - 1;
            }
            else if (rowIndex > 1) {
                prevRowIndex = rowIndex - 1;
                prevColIndex = this.columnsCount();
            }
            //verify that this is a valid cell and move further if its a colspan
            var row = this.rows().filter(function (r) { return r.position() == prevRowIndex; })[0];
            var getPrevColumn = function () {
                var column = _this.columns().filter(function (c) { return c.position() === prevColIndex; })[0];
                if (!column)
                    return;
                var item = row.values[column.path];
                if (item && item() === undefined) {
                    //Find previous column with value
                    prevColIndex--;
                    getPrevColumn();
                }
            };
            getPrevColumn();
            return { Column: prevColIndex, Row: prevRowIndex };
        };
        GridViewModel.prototype.clearSelected = function (callback) {
            this.clearCopyPasteBox();
            // clear cells
            this.originSelectedCell(null);
            this.rows().forEach(function (row) {
                return Object.keys(row.values).forEach(function (key, index) {
                    var cell = row.values[key];
                    cell.isSelected(false);
                    cell.isInCopyOrPasteGroup(false);
                });
            });
            // clear rows
            this.selectedRows().forEach(function (r) { return r.selected(false); });
            log.debug("Cleared selected cells/rows");
            if (callback && typeof callback === 'function')
                callback();
        };
        GridViewModel.prototype.selectCell = function (column, row, selectMultiple) {
            var _this = this;
            var previousCell = this.originSelectedCell();
            var isValidSelection = true;
            if (!column || !row) {
                isValidSelection = false;
            }
            else {
                if (previousCell && (previousCell.column.position == column.position && previousCell.row.position == row.position))
                    return;
            }
            // It is important that we setup the handlers before we attempt any validation on the 
            // previous cell, otherwise things get a bit awkward with checkboxes. 
            //
            // The validation code would post the server with a version of the record that did not
            // contain the result of clicking the checkbox. The record on the client would then get 
            // updated based on the click. Unfortunatly when the validation AJAX returned it would 
            // overwrite the record, resetting the value we just updated.
            if (isValidSelection) {
                var value = row.values[column.path];
                var newSelectedCell = {
                    column: column,
                    row: row,
                    value: value,
                    originalValue: pathReader.getPropertyValue(column.path, row.dirtyRecord || row.record)
                };
                if (!selectMultiple) {
                    this.clearSelected(function () {
                        _this.originSelectedCell(newSelectedCell);
                        value.isSelected(true);
                    });
                }
                else {
                    if (!previousCell) {
                        this.originSelectedCell(newSelectedCell);
                        newSelectedCell.value.isSelected(true);
                    }
                    else if (!value.isSelected()) {
                        value.isInCopyOrPasteGroup(true);
                    }
                }
            }
            if (previousCell) {
                if (!selectMultiple)
                    previousCell.value.isSelected(false);
                if (this.inEditMode()) {
                    if (this.config.validate) {
                        this.validate(previousCell.row, this.validationOnSuccess, function (validatedRow, validation) { return validatedRow.applyValidation(validation); });
                    }
                }
            }
        };
        GridViewModel.prototype.selectCellAt = function (columnPosition, rowPosition, selectMultiple) {
            var _this = this;
            if (columnPosition <= 0)
                columnPosition = 1;
            if (rowPosition <= 0)
                rowPosition = 1;
            if (columnPosition > this.columnsCount())
                columnPosition = this.columnsCount();
            if (rowPosition > this.rowCount())
                rowPosition = this.rowCount();
            //Adjust columnPosition to take colspan into account
            var row = this.rows().filter(function (r) { return r.position() == rowPosition; })[0];
            var getPrevColumn = function () {
                var column = _this.columns().filter(function (c) { return c.position() === columnPosition; })[0];
                if (!column)
                    return;
                var item = row.values[column.path];
                if (item && item() === undefined) {
                    //Find previous column with value
                    columnPosition--;
                    getPrevColumn();
                }
            };
            getPrevColumn();
            var current = this.originSelectedCell();
            var currentColumn, currentRow;
            if (current) {
                currentColumn = current.column.position();
                currentRow = current.row.position();
            }
            var newColumn = null;
            var newRow = null;
            if (currentColumn === columnPosition) {
                newColumn = current.column;
            }
            else {
                newColumn = this.columns().filter(function (c) { return c.position() === columnPosition; })[0];
            }
            if (currentRow === rowPosition) {
                newRow = current.row;
            }
            else {
                newRow = this.rows().filter(function (r) { return r.position() === rowPosition; })[0];
            }
            this.selectCell(newColumn, newRow, selectMultiple);
        };
        GridViewModel.prototype.selectCellMultiple = function (column, row) {
            var _this = this;
            if (!this.originSelectedCell()) {
                this.selectCell(column, row);
                return;
            }
            var startPosX = this.originSelectedCell().column.position();
            var startPosY = this.originSelectedCell().row.position();
            var endPosX = column.position();
            var endPosY = row.position();
            if (endPosX === startPosX && endPosY === startPosY)
                return;
            this.clearSelected(function () { return _this.selectCellsInRange(startPosX, startPosY, endPosX, endPosY); });
        };
        GridViewModel.prototype.selectCellsInRange = function (startPosX, startPosY, endPosX, endPosY) {
            var _this = this;
            var startX = startPosX;
            var startY = startPosY;
            var endX = endPosX;
            var endY = endPosY;
            //Swap if required
            if (endX < startX) {
                startX = endPosX;
                endX = startPosX;
            }
            if (endY < startY) {
                startY = endPosY;
                endY = startPosY;
            }
            var minX = startX;
            var maxX = endX;
            //Expand X if cells span columns
            this.rows().filter(function (r) { return r.position() >= startY && r.position() <= endY; }).forEach(function (row) {
                var _loop_1 = function (col) {
                    var minCol = col;
                    var getColItem = function () {
                        var column = _this.columns().filter(function (c) { return c.position() === minCol; })[0];
                        if (!column)
                            return;
                        var item = row.values[column.path];
                        if (item && item() === undefined) {
                            //Find previous column with value
                            minCol--;
                            item = getColItem();
                            if (item && item() !== undefined && minCol + item.colSpan < col) {
                                minCol = col;
                                return;
                            }
                        }
                        return item;
                    };
                    var item = getColItem();
                    maxX = Math.max(maxX, minCol + ((item && item.colSpan) ? item.colSpan : 1) - 1);
                    minX = Math.min(minX, minCol);
                };
                for (var col = startX; col <= endX; col++) {
                    _loop_1(col);
                }
            });
            for (var x = minX; x <= maxX; x++)
                for (var y = startY; y <= endY; y++)
                    this.selectCellAt(x, y, true);
        };
        GridViewModel.prototype.headingClicked = function (context, model) {
            if (this.inEditMode() || this.config.disableGridSorting())
                return;
            // currently all the heading does is toggle order bys
            var orderedBy = this.config.orderedBy();
            var index = orderedBy.map(function (o) { return o.split(' ')[0]; }).indexOf(model.path);
            if (index === -1 && !!model.path) {
                this.config.orderedBy.push(model.path);
                return;
            }
            else if (index === -1) {
                return;
            }
            if (orderedBy[index].indexOf(' desc') < 0) {
                orderedBy[index] = model.path + ' desc';
                this.config.orderedBy.notifySubscribers();
            }
            else {
                this.config.orderedBy.remove(orderedBy[index]);
            }
        };
        GridViewModel.prototype.displaySorting = function (column) {
            var _this = this;
            if (this.config.disableGridSorting() || this.config.orderedBy == null)
                return false;
            return ko.pureComputed(function () {
                var orderedBy = _this.config.orderedBy();
                var exists = orderedBy.some(function (o) { return o.split(' ')[0].indexOf(column.path) === 0; });
                return exists;
            });
        };
        GridViewModel.prototype.sortingIndex = function (column) {
            var _this = this;
            return ko.pureComputed(function () {
                var orderedBy = _this.config.orderedBy();
                if (orderedBy.length < 2)
                    return '';
                for (var i = 0; i < orderedBy.length; i++) {
                    if (orderedBy[i].split(' ')[0].indexOf(column.path) === 0)
                        return i + 1;
                }
            });
        };
        GridViewModel.prototype.sortingIconClass = function (column) {
            var _this = this;
            return ko.pureComputed(function () {
                var orderedBy = _this.config.orderedBy().map(function (o) { return o.split(' '); });
                var item = orderedBy.filter(function (o) { return o[0] == column.path; })[0];
                if (item != null && item.length === 2 && item[1] === 'desc')
                    return 'fa-caret-down';
                return 'fa-caret-up';
            });
        };
        return GridViewModel;
    }());
    exports.default = GridViewModel;
});
