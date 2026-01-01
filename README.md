# QuantumTeleportMAUI

yo, i threw this together over a couple late nights because quantum teleportation still blows my mind every time. it's a simple .net maui app that lets you play with teleporting qubit states—dial in theta/phi on the bloch sphere, crank up some noise, flip on error correction, and watch the success rate.

runs on windows, mac, android, ios—whatever maui supports. core is q# for the real quantum sim (no fake stuff), with skiasharp for the bloch viz and microcharts for the bars.

### quick peek

![main screen with bloch sphere and sliders](https://miro.medium.com/v2/resize:fit:1358/format:webp/1*e2r3wIstaLYKtPDuCm0Ytg.png)

![bloch sphere rotating](https://www.jonvet.com/static/images/quantum-state-simulator/x-to-y.gif)

### what it does
- teleport arbitrary single-qubit states (ry + rz prep)
- add depolarizing noise (slide it up and watch success drop)
- basic 3-qubit bit-flip correction (toggle it—huge difference at higher noise)
- live success/fidelity bar chart
- export run history to csv (timestamps, params, rates)
- 1000 shots default, tweak up to 5000 if your machine's beefy

### setup (dead simple)
1. grab .net 8 sdk if you don't have it
2. `dotnet workload install maui`
3. clone this repo
4. `dotnet restore`
5. run it: `dotnet run -f net8.0-windows10.0.19041.0` (or mac/android/ios)

### test the q# bit quick
`dotnet run --project QuantumLib`

### build a single exe
`dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true`

drops a fat executable—no installer needed.

### todo / ideas
- multi-qubit teleport (cat states?)
- better error correction (full steane or surface code)
- azure quantum hardware toggle
- listview for history instead of just export

bugs? weird behavior? open an issue or pr—i'm around. star if you mess with it, makes my day.

peace
