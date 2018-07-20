define(["require", "exports", "linqjs"], function (require, exports, Enumerable) {
    "use strict";
    var idEnumerator = Enumerable.toInfinity().getEnumerator();
    return {
        nextId: function () {
            idEnumerator.moveNext();
            return idEnumerator.current();
        }
    };
});
