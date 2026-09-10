// Extend the Window interface to include custom properties
declare global {
    interface Window {
        simulation: any; // Replace 'any' with a more specific type if known
        initSimulation: (instance: any) => void; // Replace 'any' with a more specific type if known
        animationFrame: number;
        pauseSimulation: () => void;
        resumeSimulation: () => void;
        simulationRunning: boolean;
    }

    interface CanvasRenderingContext2D {
        drawTriangles(triangleData: Triangle[]): void;
        drawCircles(circleData: Circle[]): void;
        clear(): void;
    }
}

const originalWidth: number = 1600; // Replace with your original width
const originalHeight: number = 800; // Replace with your original height
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

    window.simulation.canvas.width = newWidth;
    window.simulation.canvas.height = newHeight;

    let newStyleWidth: string = `${newWidth / devicePixelRatio}px`;
    let newStyleHeight: string = `${newHeight / devicePixelRatio}px`;

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
    this.fillStyle = 'rgb(6, 16, 14)';
    this.fillRect(0, 0, this.canvas.width, this.canvas.height);
}
CanvasRenderingContext2D.prototype.drawTriangles = function drawTriangles(triangleData: Triangle[]): void {
    for (const triangle of triangleData) {
        const { a, b, c } = triangle;

        this.beginPath();
        this.strokeStyle = 'rgb(143, 255, 220)';
        this.fillStyle = 'rgb(55, 214, 163)';
        this.shadowColor = 'rgba(75, 243, 188, 0.55)';
        this.shadowBlur = 8;
        this.lineWidth = 2;

        this.moveTo(a.x, a.y);
        this.lineTo(b.x, b.y);
        this.lineTo(c.x, c.y);
        this.closePath();
        this.stroke();
        this.fill();
        this.closePath();
        this.shadowBlur = 0;
    }
};

CanvasRenderingContext2D.prototype.drawCircles = function drawCircles(circleData: Circle[]): void {
    for (const circle of circleData) {
        this.beginPath();
        this.arc(circle.m.x, circle.m.y, circle.radius, 0, 2.0 * Math.PI);
        this.fillStyle = 'rgb(217, 255, 98)';
        this.shadowColor = 'rgba(217, 255, 98, 0.65)';
        this.shadowBlur = 10;
        this.fill();
        this.closePath();
        this.shadowBlur = 0;
    }
}

async function redraw(time: number): Promise<void> {
    if (!window.simulationRunning)
        return;

    try {
        const worldJsonString: string = await window.simulation.instance.invokeMethodAsync('Update', time);
        const world: RenderInformation = JSON.parse(worldJsonString);
        let canvasContext = window.simulation.canvas.getContext('2d');
        canvasContext.clear();
        canvasContext.drawCircles(world.circles);
        canvasContext.drawTriangles(world.triangles);
    } catch (e) {
        console.error('Error updating simulation:', e);
    } finally {
        if (window.simulationRunning)
            window.animationFrame = window.requestAnimationFrame(redraw);
    }
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
    window.simulationRunning = false;

    window.addEventListener("resize", onResize);
    onResize();
    window.resumeSimulation();
};

window.pauseSimulation = (): void => {
    window.simulationRunning = false;
    window.cancelAnimationFrame(window.animationFrame);
};

window.resumeSimulation = (): void => {
    if (window.simulationRunning)
        return;

    window.simulationRunning = true;
    window.animationFrame = window.requestAnimationFrame(redraw);
};

export { };
