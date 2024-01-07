﻿const originalWidth = 600; // Replace with your original width
const originalHeight = 333; // Replace with your original height
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
        // If the width-based height fits in the window, use the full width
        newWidth = maxWidth;
        newHeight = maxWidth / aspectRatio;
    } else {
        // If the width-based height is too tall, fit the canvas to the window height
        newHeight = maxHeight;
        newWidth = maxHeight * aspectRatio;
    }

    console.log('New canvas dimensions:', newWidth, newHeight);

    simulation.canvas.width = newWidth;
    simulation.canvas.height = newHeight;

    simulation.canvas.style.border = 5 / devicePixelRatio + 'px solid LightSteelBlue';
    simulation.canvas.style.margin = 10 / devicePixelRatio + 'px';

    let newStyleWidth = 0.7 * newWidth / devicePixelRatio + 'px';
    let newStyleHeight = 0.7 * newHeight / devicePixelRatio + 'px';

    console.log('New canvas style:', newStyleWidth, newStyleHeight);

    simulation.canvas.style.width = newStyleWidth;
    simulation.canvas.style.height = newStyleHeight;

    simulation.instance.invokeMethodAsync('ResizeCanvas', simulation.canvas.width, simulation.canvas.height);
}

function redraw(time) {
    window.requestAnimationFrame(redraw);
    simulation.instance.invokeMethodAsync('Loop', time);
}

window.initSimulation = (instance) => {
    let canvasContainer = document.getElementById('simulationCanvas'),
        canvases = canvasContainer.getElementsByTagName('canvas') || [];
    window.simulation = {
        instance: instance,
        canvas: canvases.length ? canvases[0] : null
    };

    window.addEventListener("resize", onResize);
    onResize();
    window.requestAnimationFrame(redraw);
};