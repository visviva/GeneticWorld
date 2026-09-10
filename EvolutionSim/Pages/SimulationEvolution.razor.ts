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
        drawCreatures(creatureData: Creature[], time: number): void;
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

interface Creature {
    m: Point;
    heading: number;
    size: number;
}

interface RenderInformation {
    creatures: Creature[];
    circles: Circle[];
}

CanvasRenderingContext2D.prototype.clear = function clearCanvas(): void {
    this.fillStyle = 'rgb(6, 16, 14)';
    this.fillRect(0, 0, this.canvas.width, this.canvas.height);
}
CanvasRenderingContext2D.prototype.drawCreatures = function drawCreatures(creatureData: Creature[], time: number): void {
    for (let index = 0; index < creatureData.length; index++) {
        const creature = creatureData[index];
        const { m, heading, size } = creature;
        const sway = Math.sin(time * 0.006 + index * 1.73) * size * 0.08;

        this.save();
        this.translate(m.x, m.y);
        this.rotate(heading);
        this.lineCap = 'round';
        this.lineJoin = 'round';

        // Four trailing feelers give the silhouette an organic, jellyfish-like motion.
        this.beginPath();
        this.strokeStyle = 'rgba(75, 243, 188, 0.68)';
        this.lineWidth = Math.max(1.2, size * 0.045);
        for (const side of [-1, 1]) {
            this.moveTo(side * size * 0.10, -size * 0.23);
            this.quadraticCurveTo(side * size * 0.18 - sway * 0.35, -size * 0.43, side * size * 0.10 + sway, -size * 0.62);
            this.moveTo(side * size * 0.22, -size * 0.13);
            this.quadraticCurveTo(side * size * 0.40 + sway * 0.2, -size * 0.32, side * size * 0.42 - sway * 0.55, -size * 0.47);
        }
        this.stroke();

        // These two long sensory tentacles replace the triangle's forward tip.
        this.beginPath();
        this.strokeStyle = 'rgb(126, 255, 215)';
        this.lineWidth = Math.max(1.4, size * 0.055);
        for (const side of [-1, 1]) {
            this.moveTo(side * size * 0.13, size * 0.22);
            this.bezierCurveTo(
                side * size * 0.20,
                size * 0.44,
                side * size * 0.30 + sway * 0.35,
                size * 0.68,
                side * size * 0.25 + sway * 0.18,
                size * 0.88);
        }
        this.stroke();

        // Small luminous receptors make the forward direction readable at a glance.
        for (const side of [-1, 1]) {
            this.beginPath();
            this.arc(side * size * 0.25 + sway * 0.18, size * 0.88, Math.max(1.5, size * 0.055), 0, Math.PI * 2);
            this.fillStyle = 'rgb(217, 255, 98)';
            this.fill();
        }

        // Compact oval body with a soft bioluminescent shell.
        this.beginPath();
        this.shadowColor = 'rgba(75, 243, 188, 0.5)';
        this.shadowBlur = 7;
        this.ellipse(0, 0, size * 0.30, size * 0.36, 0, 0, Math.PI * 2);
        this.fillStyle = 'rgb(38, 183, 139)';
        this.fill();
        this.strokeStyle = 'rgb(155, 255, 224)';
        this.lineWidth = Math.max(1.3, size * 0.05);
        this.stroke();
        this.shadowBlur = 0;

        // A translucent inner membrane and three dark sensory eyes add alien detail.
        this.beginPath();
        this.ellipse(0, -size * 0.035, size * 0.19, size * 0.24, 0, 0, Math.PI * 2);
        this.fillStyle = 'rgba(174, 255, 226, 0.2)';
        this.fill();

        for (const eyeX of [-0.13, 0, 0.13]) {
            this.beginPath();
            this.ellipse(size * eyeX, size * 0.11, Math.max(1.1, size * 0.035), Math.max(1.6, size * 0.055), 0, 0, Math.PI * 2);
            this.fillStyle = 'rgb(5, 31, 25)';
            this.fill();
        }

        this.restore();
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
        canvasContext.drawCreatures(world.creatures, time);
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
