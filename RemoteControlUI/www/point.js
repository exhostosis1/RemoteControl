const CLICKINTERVAL = 200;
const CLICKRANGE = 10;

class Point {
    constructor(x, y, id = 0) {
        this.x = Math.round(x);
        this.y = Math.round(y);
        this.id = id;
        this.start = new Date();
    }    

    checkRange = function (that) {
        return  Math.abs(this.x - that.x) <= CLICKRANGE &&
                Math.abs(this.y - that.y) <= CLICKRANGE;
    }

    checkTime = function (that) {
        return that.start - this.start <= CLICKINTERVAL;
    }
}

export { Point };