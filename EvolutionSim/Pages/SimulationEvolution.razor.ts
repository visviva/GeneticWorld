// Extend the Window interface to include custom properties
declare global {
    interface Window {
        simulation: any; // Replace 'any' with a more specific type if known
        initSimulation: (instance: any) => void; // Replace 'any' with a more specific type if known
    }

    interface CanvasRenderingContext2D {
        drawTriangles(triangleData: Triangle[]): void;
        drawCircles(circleData: Circle[]): void;
        clear(): void;
    }
}

const originalWidth: number = 600; // Replace with your original width
const originalHeight: number = 333; // Replace with your original height
const aspectRatio: number = originalWidth / originalHeight;

function onResize(): void {
    let container: HTMLElement | null = document.getElementById("simulationCanvas");
    if (!window.simulation.canvas || !container)
        return;

    let devicePixelRatio: number = window.devicePixelRatio || 1; // Get the device pixel ratio

    // Calculate the new dimensions based on the aspect ratio and available window size
    let maxWidth: number = container.clientWidth * devicePixelRatio;
    let maxHeight: number = window.innerHeight * devicePixelRatio;
    let newWidth: number, newHeight: number;

    if (maxWidth / aspectRatio <= maxHeight) {
        newWidth = maxWidth;
        newHeight = maxWidth / aspectRatio;
    } else {
        newHeight = maxHeight;
        newWidth = maxHeight * aspectRatio;
    }

    console.log('New canvas dimensions:', newWidth, newHeight);

    window.simulation.canvas.width = newWidth;
    window.simulation.canvas.height = newHeight;

    window.simulation.canvas.style.border = `${5 / devicePixelRatio}px solid LightSteelBlue`;
    window.simulation.canvas.style.margin = `${10 / devicePixelRatio}px`;

    let newStyleWidth: string = `${0.7 * newWidth / devicePixelRatio}px`;
    let newStyleHeight: string = `${0.7 * newHeight / devicePixelRatio}px`;

    console.log('New canvas style:', newStyleWidth, newStyleHeight);

    window.simulation.canvas.style.width = newStyleWidth;
    window.simulation.canvas.style.height = newStyleHeight;

    window.simulation.instance.invokeMethodAsync('ResizeCanvas', window.simulation.canvas.width, window.simulation.canvas.height);
}

interface Point {
    x: number;
    y: number;
}

interface Circle {
    m: Point;
    radius: number;
}

interface Triangle {
    a: Point;
    b: Point;
    c: Point;
}

interface RenderInformation {
    triangles: Triangle[];
    circles: Circle[];
}

CanvasRenderingContext2D.prototype.clear = function clearCanvas(): void {
    this.fillStyle = 'rgb(250, 250, 255)';
    this.fillRect(0, 0, this.canvas.width, this.canvas.height);
}
CanvasRenderingContext2D.prototype.drawTriangles = function drawTriangles(triangleData: Triangle[]): void {
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

CanvasRenderingContext2D.prototype.drawCircles = function drawCircles(circleData: Circle[]): void {
    for (const circle of circleData) {
        this.beginPath();
        this.arc(circle.m.x, circle.m.y, circle.radius, 0, 2.0 * Math.PI);
        this.fillStyle = 'rgb(148, 157, 106)';
        this.fill();
        this.closePath();
    }
}

function redraw(time: number): void {
    window.requestAnimationFrame(redraw);
    const newWorld: Promise<string> = window.simulation.instance.invokeMethodAsync('Update', time);
    newWorld.then((worldJsonString: string) => {
        try {
            const world: RenderInformation = JSON.parse(worldJsonString);
            let canvasContext = window.simulation.canvas.getContext('2d');
            canvasContext.clear();
            canvasContext.drawCircles(world.circles);
            canvasContext.drawTriangles(world.triangles);
        } catch (e) {
            console.error('Error parsing JSON:', e);
        }
    });
}

window.initSimulation = (instance: any): void => {
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

export { };
