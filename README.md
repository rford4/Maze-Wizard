# MazeWizard

.NET 10 Console app that solves rectangular mazes with orthogonal (square cell) pathways.

The maze must follow these pixel-color rules:
- Entrance:    red pixels      (RGB 255, 0, 0)
- Exit:        blue pixels     (RGB 0, 0, 255)
- Walls:       black pixels    (RGB 0, 0, 0)
   
The maze must also be fully surrounded by black walls.

The solution to the maze, if found, will be painted in green.

## Dependencies

> [!IMPORTANT]
> Current version of the application relies on the Windows GDI for image processing and can only run in Windows runtime environments.

## Usage

mazewizard [\<source\> [\<destination\>]] [options]

## Arguments
| Argument | Description |
|----------|-------------|
| `\<source\>` | The maze image to solve. Supported file types: ```[bmp, jpg, png]```.<br/> |
| `\<destination\>` | The ouput image to write. Supported file types: ```[bmp, jpg, png]```. |

## Options
| Option | Description |
|----------|-------------|
| `--o, --overwrite`   | Allow overwriting the destination file if it already exists. |
| `-?, -h, --help` | Show help and usage information. |
| `--version` | Show version information. |
