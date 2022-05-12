# Emission Settings

* [Emission Map](#emission-map)
* [Base Speed (Time)](#base-speed-time)
* [Animation Mode](#animation-mode)
* [Scroll U/X direction](#scroll-ux-direction)
* [Scroll V/Y direction](#scroll-vy-direction)
* [Rotate around UV center](#rotate-around-uv-center)
* [Ping-pong moves for base](#ping-pong-moves-for-base)

* [Color Shifting with Time](#color-shifting-speed-time)
* [Destination Color](#destination-color)
* [Color Shifting Speed (Time)](#color-shifting-speed-time)

* [Color Shifting with View Angle](#color-shifting-with-view-angle)
* [Shifting Target Color](#shifting-target-color)


### Emission Map
Primarily used with the Bloom Post Effect, Luminous objects can be represented.

### Base Speed (Time)
Specifies the base update speed of scroll animation. If the value is 1, it will be updated in 1 second. Specifying a value of 2 results in twice the speed of a value of 1, so it will be updated in 0.5 seconds.

### Animation Mode
Controls the animated scrolling of the emissive texture.

### Scroll U/X direction
Specifies how much the Emissive texture should scroll in the u-direction (x-axis direction) when updating the animation. The scrolling animation is ultimately determined by Base Speed (Time) x Scroll U Direction x Scroll V Direction.

### Scroll V/Y direction
Specifies how much the Emissive texture should scroll in the V-direction (y-axis direction) when updating the animation. The scrolling animation is ultimately determined by Base Speed (Time) x Scroll U Direction x Scroll V Direction.

### Rotate around UV center
When Base Speed=1, the Emissive texture will rotate clockwise by 1. When combined with scrolling, rotation will occur after scrolling.

### Ping-pong moves for base
When enabled, you can set PingPong (back and forth) in the direction of the animation.

### Color Shifting with Time
The color that is multiplied by the Emissive texture is changed by linear interpolation (Lerp) toward the Destination Color.

### Destination Color
Destination color above, must be specified in HDR.

### Color Shifting Speed (Time)
Sets the reference speed for color shift. When the value is 1, one cycle should take approximately 6 seconds.

### Color Shifting with View Angle
Emissive color shifts in accordance with view angle.

### Shifting Target Color
Target color for Color Shifting with View Angle which must be specified in HDR.