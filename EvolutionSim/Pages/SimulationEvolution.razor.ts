// Extend the Window interface to include custom properties
declare global {
    interface Window {
        simulation: any; // Replace 'any' with a more specific type if known
        initSimulation: (instance: any) => void; // Replace 'any' with a more specific type if known
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

function clearCanvas(canvas: HTMLCanvasElement): void {
    const ctx: CanvasRenderingContext2D = canvas.getContext('2d')!;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
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

function drawTriangles(triangleData: Triangle[], ctx: CanvasRenderingContext2D): void {
    for (const triangle of triangleData) {
        const { a, b, c } = triangle;

        ctx.beginPath();
        ctx.strokeStyle = '#ff0000';
        ctx.fillStyle = '#000000';
        ctx.lineWidth = 3;

        ctx.moveTo(a.x, a.y);
        ctx.lineTo(b.x, b.y);
        ctx.lineTo(c.x, c.y);
        ctx.closePath();
        ctx.stroke();
        ctx.fill();
    }
}

function drawCircles(circleData: Circle[], ctx: CanvasRenderingContext2D): void {
    for (const circle of circleData) {
        ctx.beginPath();
        ctx.arc(circle.m.x, circle.m.y, circle.radius, 0, 2.0 * Math.PI);
        ctx.fillStyle = 'rgb(0,0,0)';
        ctx.fill();
        ctx.closePath();
    }
}

function redraw(time: number): void {
    window.requestAnimationFrame(redraw);
    const newWorld: Promise<string> = window.simulation.instance.invokeMethodAsync('Update', time);
    newWorld.then((worldJsonString: string) => {
        try {
            const world: RenderInformation = JSON.parse(worldJsonString);
            clearCanvas(window.simulation.canvas);
            drawTriangles(world.triangles, window.simulation.canvas.getContext('2d')!);
            drawCircles(world.circles, window.simulation.canvas.getContext('2d')!);
        } catch (e) {
            console.error('Error parsing JSON:', e);
        }
    });
}

window.initSimulation = (instance: any): void => {
    let canvasContainer: HTMLElement | null = document.getElementById('simulationCanvas');
    let canvases: HTMLCollectionOf<HTMLCanvasElement> = canvasContainer!.getElementsByTagName('canvas');
    window.simulation = {
        instance: instance,
        canvas: canvases.length ? canvases[0] : null
    };

    window.addEventListener("resize", onResize);
    onResize();
    redraw(0.0);
};

export { };
