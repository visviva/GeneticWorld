const viewScale = window.devicePixelRatio || 1;
const originalWidth = 600; // Replace with your original width
const originalHeight = 333.333; // Replace with your original height
const aspectRatio = originalWidth / originalHeight;
function onResize() {
    let container = document.getElementById("simulationCanvas");
    if (!window.simulation.canvas || !container)
        return;

    let devicePixelRatio = window.devicePixelRatio || 1; // Get the device pixel ratio

    // Calculate the new dimensions based on the aspect ratio and available window size
    let maxWidth = 0.7 * container.clientWidth * devicePixelRatio;
    let maxHeight = 0.7 * window.innerHeight * devicePixelRatio;
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


    let newStyleWidth = newWidth / devicePixelRatio + 'px';
    let newStyleHeight = newHeight / devicePixelRatio + 'px';

    console.log('New canvas style:', newStyleWidth, newStyleHeight);

    simulation.canvas.style.width = newStyleWidth;
    simulation.canvas.style.height = newStyleHeight;

    simulation.instance.invokeMethodAsync('ResizeCanvas', simulation.canvas.width, simulation.canvas.height);
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
};
