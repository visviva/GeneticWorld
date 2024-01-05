using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Drawing;
using System.Data;
using GeneticWorld.Core;

namespace GeneticWorld.Components.Pages;

public partial class Home
{
    BECanvasComponent? _canvasReference = null;
    Canvas2DContext? _context = null;

    readonly Sprite _sprite = new();
    Point _spritePosition = Point.Empty;
    Point _spriteDirection = new(1, 1);
    readonly float _spriteSpeed = 0.25f;
    readonly Time _time = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        /*        if (firstRender)
                {
                    await JSRuntime.InvokeVoidAsync("canvasInterop.setupCanvas");

                    _context = await _canvasReference.CreateCanvas2DAsync();

                    _sprite.Size = await GetImageSize(_sprite.SpriteSheet);

                    await JSRuntime.InvokeAsync<object>("initGame", DotNetObjectReference.Create(this));
                }*/
    }

    private async Task<Size> GetImageSize(ElementReference imageElement)
    {
        return await JSRuntime.InvokeAsync<Size>("getElementSize", imageElement);
    }

    [JSInvokable]
    public async ValueTask GameLoop(float timeStamp, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(_context);

        _time.TotalTime = timeStamp;

        Update(width, height);
        await Render(width, height);
    }

    private void Update(int width, int height)
    {
        if ((_spritePosition.X + _sprite.Size.Width >= width - 1) || _spritePosition.X < 0)
        {
            _spriteDirection.X = -_spriteDirection.X;
        }

        if ((_spritePosition.Y + _sprite.Size.Height >= height - 1) || _spritePosition.Y < 0)
        {
            _spriteDirection.Y = -_spriteDirection.Y;
        }

        _spritePosition.X += (int)(_spriteDirection.X * _spriteSpeed * _time.ElapsedTime);
        _spritePosition.Y += (int)(_spriteDirection.Y * _spriteSpeed * _time.ElapsedTime);
    }
    private async Task Render(int width, int height)
    {
        ArgumentNullException.ThrowIfNull(_context);

        await _context.ClearRectAsync(0, 0, width, height);
        await _context.DrawImageAsync(_sprite.SpriteSheet, _spritePosition.X, _spritePosition.Y, _sprite.Size.Width, _sprite.Size.Height);
    }
}
