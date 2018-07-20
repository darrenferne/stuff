define(["require", "exports", "knockout", "options", "sprintf", "loglevel"], function (require, exports, ko, options, sprintfM, log) {
    "use strict";
    var sprintf = sprintfM.sprintf;
    var EmptyCell = /** @class */ (function () {
        function EmptyCell(row, column) {
            this.isEditable = ko.observable(false);
            this.isFocused = ko.pureComputed(function () { return false; });
            this.isSelected = ko.observable(true);
            this.isInCopyOrPasteGroup = ko.observable(true);
            this.isDirty = ko.pureComputed(function () { return false; });
            this.isValid = ko.observable(true);
            this.colSpan = 0;
            this.doHighlight = function () { };
            this.getClipboardValue = function () { return ""; };
            this.row = row;
            this.column = column;
        }
        return EmptyCell;
    }());
    var Clipboard = /** @class */ (function () {
        function Clipboard() {
        }
        Clipboard.getPasteDataType = function (data) {
            var hasNoTabs = data.indexOf(this.tabChar) === -1;
            var hasTabs = !hasNoTabs;
            var hasNoNewLines = data.indexOf(this.newLineChar) === -1;
            var hasNewLines = !hasNoNewLines;
            var hasNewlineAtEnd = data.indexOf(this.newLineChar) === data.length - 1;
            // if we don't have tab seperated data and it either has no new line OR 
            // ends with a new line(as this is common when copying [copying a single
            // cell or row from Excel will do this])
            if (hasNoTabs && (hasNoNewLines || hasNewlineAtEnd))
                return Clipboard.PasteDataType.String;
            else if (hasNewLines || hasTabs)
                return Clipboard.PasteDataType.Rows;
            else
                return Clipboard.PasteDataType.Unknown;
        };
        Clipboard.getCellsAsArrayOfRows = function () {
            var rowCells = $.makeArray($('.active-bwf-grid .bwf-grid .bwf-cell'))
                .map(function (x) { return ko.contextFor(x).$data; })
                .reduce(function (acc, item) {
                if (!acc[item.row.position()]) {
                    acc[item.row.position()] = [];
                    acc[item.row.position()].row = item.row;
                }
                acc[item.row.position()].push(item);
                for (var c = 1; c < item.colSpan; ++c)
                    acc[item.row.position()].push(new EmptyCell(item.row, item.column));
                return acc;
            }, [])
                .map(function (row) { return row.sort(function (l, r) { return l.column.position() - r.column.position(); }); })
                .sort(function (l, r) { return l.row.position() - r.row.position(); });
            return rowCells;
        };
        Clipboard.getSelectedCellDataAs2DArray = function () {
            var arrayFiltered = this.getCellsAsArrayOfRows()
                .filter(function (ro) { return ro.some(function (r) { return r.isSelected() || r.isInCopyOrPasteGroup(); }); });
            arrayFiltered.forEach(function (row) {
                row.forEach(function (val, colIndex) {
                    if (!(val.isInCopyOrPasteGroup() || val.isSelected()))
                        row[colIndex] = null;
                });
            });
            var startColumnIndexWithData = -1, endColumnIndexWithData = -1;
            for (var row = 0; row < arrayFiltered.length; row++) {
                // get which column to start getting data from
                for (var colIndex = 0; colIndex < arrayFiltered[row].length; colIndex++) {
                    if (arrayFiltered[row][colIndex] == null)
                        continue;
                    if (startColumnIndexWithData == -1 || (startColumnIndexWithData > colIndex)) {
                        startColumnIndexWithData = colIndex;
                        break;
                    }
                }
                // get which column to end getting data from
                for (var colIndex = arrayFiltered[row].length - 1; colIndex >= startColumnIndexWithData; colIndex--) {
                    if (arrayFiltered[row][colIndex] == null)
                        continue;
                    if (colIndex > endColumnIndexWithData) {
                        endColumnIndexWithData = colIndex;
                        break;
                    }
                }
            }
            // remove extra columns
            arrayFiltered = arrayFiltered.map(function (row) { return row.filter(function (x, index) { return index >= startColumnIndexWithData && index <= endColumnIndexWithData; }); });
            var returnData = {
                data: arrayFiltered,
                startColumn: startColumnIndexWithData
            };
            return returnData;
        };
        Clipboard.getSelectedRowDataAs2DArray = function () {
            return this.getCellsAsArrayOfRows()
                .filter(function (x) { return x.row.selected(); })
                .filter(function (x) { return x.filter(function (y) { return y !== null; }).length > 0; });
        };
        Clipboard.getCopiedData = function (canSelectIndividualCells, rows, columns) {
            var _this = this;
            var dataInExcelForm = "";
            var array;
            if (canSelectIndividualCells) {
                var selectedDataCellsArray = this.getSelectedCellDataAs2DArray();
                if (selectedDataCellsArray.data.length === 0)
                    return null;
                array = selectedDataCellsArray.data;
            }
            else {
                var selectedDataRowsArray = this.getSelectedRowDataAs2DArray();
                if (!selectedDataRowsArray)
                    return null;
                array = selectedDataRowsArray;
            }
            array.forEach(function (row, index) {
                var rowString = "";
                row.forEach(function (item, colIndex) {
                    if (item != null) {
                        var valueFormatted = item.getClipboardValue();
                        if (valueFormatted != null)
                            rowString += valueFormatted.toString().replace(/[\r\n\t]/, '');
                    }
                    // we don't want tabs at the end of lines or a newline at the end of the last line
                    // as it means we might overwrite other data in the worksheet as it adds an extra
                    // column to the right of the data and an extra row
                    if (colIndex != row.length - 1)
                        rowString += _this.tabChar;
                });
                dataInExcelForm += rowString;
                if (index != array.length - 1)
                    dataInExcelForm += _this.newLineChar;
            });
            return dataInExcelForm;
        };
        // -----------------------------------
        // PASTE METHODS
        // -----------------------------------
        Clipboard.processPastedData = function (grid, data) {
            if (!grid.inEditMode())
                return;
            // if no data or if we only have tabs/new lines
            if (!data || !(data.replace(/[\r\n\t]/, '')))
                return;
            var selected = grid.originSelectedCell();
            var pasteInProgressFlag;
            if (grid.flags && grid.flags.pasteInProgress)
                pasteInProgressFlag = grid.flags.pasteInProgress;
            else
                pasteInProgressFlag = ko.observable(false);
            var dataType = this.getPasteDataType(data);
            var hasNewlineAtEnd = data.indexOf(this.newLineChar) === data.length - 1;
            switch (dataType) {
                case Clipboard.PasteDataType.String:
                    // remove trailing newline if we have one
                    if (hasNewlineAtEnd)
                        data = data.substr(0, data.length - 1);
                    if (!grid.canSelectIndividualCells() || !selected) {
                        this.pasteStringErrorMessage();
                        pasteInProgressFlag(false);
                        return;
                    }
                    this.processPastedCell(grid, data, selected.row, selected.column, true);
                    if (grid.config && grid.config.updateDirtyRecordWithLatestValues)
                        grid.config.updateDirtyRecordWithLatestValues(selected.row, grid.columns());
                    pasteInProgressFlag(false);
                    break;
                case Clipboard.PasteDataType.Rows:
                    this.processPastedDataRows(grid, data);
                    grid.clearSelected();
                    break;
                case Clipboard.PasteDataType.Unknown:
                default:
                    this.unknownDataFormatMessage();
                    pasteInProgressFlag(false);
                    break;
            }
            // After we have finished pasting, reselect the origin cell
            // fixes the cell being focused but not 'selected'
            if (selected)
                grid.selectCellAt(selected.column.position(), selected.row.position());
        };
        Clipboard.actionIsNotValid = function (grid, data, leftHandColumn, originRow) {
            var _this = this;
            if (grid.canInsertRows()) {
                var totalRows = grid.recordsCount();
                var rowsToCreate = (originRow + data.length - 1) - totalRows;
                if (totalRows + rowsToCreate > grid.maxRows) {
                    this.tooManyRows();
                    return true;
                }
            }
            var dataWidths = data.map(function (d) { return d.split(_this.tabChar).length; });
            var rightHandColumn = Math.max.apply(Math, dataWidths) + leftHandColumn;
            var columns = grid.columns();
            var pasteIsTooWide = dataWidths.some(function (w) { return w + leftHandColumn > columns.length; });
            if (pasteIsTooWide) {
                this.tooManyColumnsMessage();
                return true;
            }
            var columnsInAction = columns.filter(function (c) { return c.position() > leftHandColumn && c.position() <= rightHandColumn; });
            var allColumnsAreReadOnly = columnsInAction.every(function (c) {
                return c.metadata.isNotEditableInGrid;
            });
            if (allColumnsAreReadOnly) {
                this.noColumnIsEditableMessage();
                return true;
            }
            return false;
        };
        Clipboard.processPastedDataRows = function (grid, data) {
            var _this = this;
            var dataRows = data.split(this.newLineChar).filter(function (x) { return !!x; });
            if (dataRows.length === 0)
                return;
            var selectedCell = grid.originSelectedCell();
            var totalRows = grid.recordsCount
                ? grid.recordsCount()
                : grid.rows().length;
            var startColumnPosition = 0;
            // positions start at 1
            var startRowPosition = totalRows + 1;
            // anything selected
            if (selectedCell || grid.selectedRows().length > 0) {
                var userSelectedCells = grid.userSelectedCells();
                if (grid.canSelectIndividualCells() && userSelectedCells.length > 1) {
                    startColumnPosition = Math.min.apply(Math, userSelectedCells.map(function (x) { return x.column.position(); })) - 1;
                    startRowPosition = Math.min.apply(Math, userSelectedCells.map(function (x) { return x.row.position(); }));
                }
                else if (grid.canSelectIndividualCells() && selectedCell) {
                    startColumnPosition = selectedCell.column.position() - 1;
                    startRowPosition = selectedCell.row.position();
                }
                else
                    startRowPosition = grid.selectedRows()[0].position();
            }
            if (this.actionIsNotValid(grid, dataRows, startColumnPosition, startRowPosition)) {
                if (grid.flags && grid.flags.pasteInProgress)
                    grid.flags.pasteInProgress(false);
                return;
            }
            var rowsToCreate = (startRowPosition + dataRows.length - 1) - totalRows;
            var sub = null;
            var doPaste = function (newRows) {
                dataRows.forEach(function (val, index) {
                    if ((index + startRowPosition) > totalRows + newRows)
                        return;
                    var row = grid.rows().filter(function (x) { return x.position() === startRowPosition + index; })[0];
                    _this.processPastedDataRow(grid, val, row, startColumnPosition);
                    if (grid.config && grid.config.updateDirtyRecordWithLatestValues)
                        grid.config.updateDirtyRecordWithLatestValues(row, grid.columns());
                });
                if (grid.flags && grid.flags.pasteInProgress)
                    grid.flags.pasteInProgress(false);
            };
            if (rowsToCreate <= 0 || !grid.canInsertRows()) {
                doPaste(0);
                return;
            }
            var rendered = 0;
            var waitingOn = [];
            var topic = sprintf("%s-rendered-row", grid.viewGridId);
            var onRendered = function (bwfId) {
                if (waitingOn.some(function (w) { return w === bwfId; })) {
                    rendered++;
                    if (rendered === rowsToCreate) {
                        sub.dispose();
                        doPaste(rowsToCreate);
                    }
                }
            };
            sub = ko.postbox.subscribe(topic, onRendered);
            for (var r = 1; r <= rowsToCreate; r++) {
                var record = grid.insertRecordAtBottom();
                waitingOn.push(record.bwfId);
            }
            return;
        };
        Clipboard.processPastedDataRow = function (grid, dataRow, row, startColumn) {
            if (startColumn === void 0) { startColumn = 0; }
            var dataRowValues = dataRow.split(this.tabChar);
            var columns = grid.columns();
            for (var i = 0; i < dataRowValues.length; i++) {
                var c = columns[i + startColumn];
                this.processPastedCell(grid, dataRowValues[i], row, c);
            }
        };
        Clipboard.processPastedCell = function (grid, data, row, column, showErrorMessageIfFail) {
            if (showErrorMessageIfFail === void 0) { showErrorMessageIfFail = false; }
            var isTypeChoice = column.metadata._isType || column.metadata.hasChoice;
            var notLink = column.metadata.type != 'link';
            var notDownload = column.metadata.type != 'download';
            var isValidEditableCell = !column.metadata.isNotEditableInGrid && (notLink || notDownload) && !row.values[column.path].isReadonly();
            if (isTypeChoice || isValidEditableCell) {
                var subscriptionString = sprintf("%s-update-cell-%d-%d", grid.viewGridId, row.position(), column.position());
                ko.postbox.publish(subscriptionString, this.getValue(data, column));
            }
            else {
                log.debug(sprintf("Attempted to paste into column '%s' | isTypeChoice = %s | isValidEditableCell = %s", column.path, isTypeChoice, isValidEditableCell));
                if (showErrorMessageIfFail)
                    this.columnIsNotEditableMessage(column.displayName());
            }
        };
        Clipboard.getValue = function (data, column) {
            var value;
            switch (column.metadata.type.toLowerCase()) {
                case "boolean":
                    var dataLower = data.toLowerCase();
                    if (this.validTrueValues.indexOf(dataLower) !== -1)
                        value = true;
                    else if (this.validFalseValues.indexOf(dataLower) !== -1)
                        value = false;
                    break;
                case "numeric":
                case "integer":
                    var numberGroupSeperator = ",", decimalSeperator = ".";
                    var kendoCulture = kendo.cultures[options.formattingCulture];
                    if (kendoCulture) {
                        numberGroupSeperator = kendoCulture.numberFormat[","];
                        decimalSeperator = kendoCulture.numberFormat["."];
                    }
                    var replaceRegex = new RegExp(sprintf('[\t\r\n%s]', numberGroupSeperator), 'g');
                    value = data.replace(replaceRegex, '');
                    break;
                default:
                    value = data.replace(/[\t\r\n]/g, '');
                    break;
            }
            return value;
        };
        // --------------
        // Error Messages
        // --------------
        Clipboard.publishError = function (message) {
            ko.postbox.publish("bwf-transient-notification", {
                message: message,
                styleClass: this.errorStyleClass
            });
        };
        Clipboard.tooManyColumnsMessage = function () {
            this.publishError(options.resources["bwf_paste_too_many_columns"]);
        };
        Clipboard.columnIsNotEditableMessage = function (path) {
            this.publishError(sprintf("%s: %s", options.resources["bwf_paste_column_not_editable"], path));
        };
        Clipboard.noColumnIsEditableMessage = function () {
            this.publishError(options.resources["bwf_paste_no_columns_editable"]);
        };
        Clipboard.unknownDataFormatMessage = function () {
            this.publishError(options.resources["bwf_paste_unknown_data_format"]);
        };
        Clipboard.pasteStringErrorMessage = function () {
            this.publishError(options.resources["bwf_paste_string_error"]);
        };
        Clipboard.tooManyRows = function () {
            this.publishError(options.resources["bwf_grid_too_many_rows"]);
        };
        Clipboard.errorStyleClass = 'alert-warning';
        Clipboard.validTrueValues = ["true", "1"];
        Clipboard.validFalseValues = ["false", "0", "null", "", null, undefined];
        Clipboard.tabChar = '\t';
        Clipboard.newLineChar = '\n';
        return Clipboard;
    }());
    (function (Clipboard) {
        var PasteDataType;
        (function (PasteDataType) {
            PasteDataType[PasteDataType["Unknown"] = -1] = "Unknown";
            PasteDataType[PasteDataType["String"] = 0] = "String";
            PasteDataType[PasteDataType["Rows"] = 1] = "Rows";
        })(PasteDataType = Clipboard.PasteDataType || (Clipboard.PasteDataType = {}));
    })(Clipboard || (Clipboard = {}));
    return Clipboard;
});
