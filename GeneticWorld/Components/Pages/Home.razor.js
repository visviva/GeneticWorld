window.canvasInterop = {
    setupCanvas: function () {
        let canvas = document.getElementById("myCanvas");
        let context = canvas.getContext("2d");
        // Perform various operations on the canvas using the context object
        // Example: Draw a rectangle
        context.fillStyle = "red";
        context.fillRect(10, 10, 100, 100);
    }
};

function gameLoop(timeStamp) {
    window.requestAnimationFrame(gameLoop);
    game.instance.invokeMethodAsync('GameLoop', timeStamp, game.canvas.width, game.canvas.height);
}

function onResize() {
    let container = document.getElementById("theCanvas");
    if (!window.game.canvas || !container)
        return;

    game.canvas.width = container.clientWidth;
    game.canvas.height = container.clientHeight;
}

window.initGame = (instance) => {
    let canvasContainer = document.getElementById('theCanvas'),
        canvases = canvasContainer.getElementsByTagName('canvas') || [];
    window.game = {
        instance: instance,
        canvas: canvases.length ? canvases[0] : null
    };

    window.addEventListener("resize", onResize);
    onResize();

    window.requestAnimationFrame(gameLoop);
};

window.getElementSize = async (element) => {
    return { width: element.naturalWidth, height: element.naturalHeight };
};


