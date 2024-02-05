const originalWidth = 1600; // Replace with your original width
const originalHeight = 800; // Replace with your original height
const aspectRatio = originalWidth / originalHeight;
function onResize() {
    let container = document.getElementById("simulationCanvas");
    if (!window.simulation.canvas || !container)
        return;
    let devicePixelRatio = window.devicePixelRatio || 1; // Get the device pixel ratio
    // Calculate the new dimensions based on the aspect ratio and available window size
    let maxWidth = container.clientWidth * devicePixelRatio;
    let maxHeight = window.innerHeight * devicePixelRatio;
    let newWidth, newHeight;
    if (maxWidth / aspectRatio <= maxHeight) {
        newWidth = maxWidth;
        newHeight = maxWidth / aspectRatio;
    }
    else {
        newHeight = maxHeight;
        newWidth = maxHeight * aspectRatio;
    }
    console.log('New canvas dimensions:', newWidth, newHeight);
    window.simulation.canvas.width = newWidth;
    window.simulation.canvas.height = newHeight;
    window.simulation.canvas.style.border = `${5 / devicePixelRatio}px solid LightSteelBlue`;
    window.simulation.canvas.style.margin = `${10 / devicePixelRatio}px`;
    let newStyleWidth = `${0.7 * newWidth / devicePixelRatio}px`;
    let newStyleHeight = `${0.7 * newHeight / devicePixelRatio}px`;
    console.log('New canvas style:', newStyleWidth, newStyleHeight);
    window.simulation.canvas.style.width = newStyleWidth;
    window.simulation.canvas.style.height = newStyleHeight;
    window.simulation.instance.invokeMethodAsync('ResizeCanvas', window.simulation.canvas.width, window.simulation.canvas.height);
}
CanvasRenderingContext2D.prototype.clear = function clearCanvas() {
    this.fillStyle = 'rgb(250, 250, 255)';
    this.fillRect(0, 0, this.canvas.width, this.canvas.height);
};
CanvasRenderingContext2D.prototype.drawTriangles = function drawTriangles(triangleData) {
    for (const triangle of triangleData) {
        const { a, b, c } = triangle;
        this.beginPath();
        this.strokeStyle = 'rgb(59, 31, 43)';
        this.fillStyle = 'rgb(70, 99, 101)';
        this.lineWidth = 3;
        this.moveTo(a.x, a.y);
        this.lineTo(b.x, b.y);
        this.lineTo(c.x, c.y);
        this.closePath();
        this.stroke();
        this.fill();
        this.closePath();
    }
};
CanvasRenderingContext2D.prototype.drawCircles = function drawCircles(circleData) {
    for (const circle of circleData) {
        this.beginPath();
        this.arc(circle.m.x, circle.m.y, circle.radius, 0, 2.0 * Math.PI);
        this.fillStyle = 'rgb(148, 157, 106)';
        this.fill();
        this.closePath();
    }
};
function redraw(time) {
    window.resumeSimulation();
    const newWorld = window.simulation.instance.invokeMethodAsync('Update', time);
    newWorld.then((worldJsonString) => {
        try {
            const world = JSON.parse(worldJsonString);
            let canvasContext = window.simulation.canvas.getContext('2d');
            canvasContext.clear();
            canvasContext.drawCircles(world.circles);
            canvasContext.drawTriangles(world.triangles);
        }
        catch (e) {
            console.error('Error parsing JSON:', e);
        }
    });
}
window.initSimulation = (instance) => {
    let theCanvas = document.getElementById('theCanvas');
    if (theCanvas === null) {
        console.log("The canvas has a problem and is null!");
        return;
    }
    window.simulation = {
        instance: instance,
        canvas: theCanvas
    };
    window.addEventListener("resize", onResize);
    onResize();
    redraw(0.0);
};
window.pauseSimulation = () => {
    window.cancelAnimationFrame(window.animationFrame);
};
window.resumeSimulation = () => {
    window.animationFrame = window.requestAnimationFrame(redraw);
};
export {};
//# sourceMappingURL=SimulationEvolution.razor.js.map