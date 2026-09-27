"""Generate an original, seamless equirectangular studio reflection map.
A lighting aid, not a photograph or a physically captured HDR environment.
Only Python's standard library is needed. Re-run from any directory.
"""
from pathlib import Path
import math, struct, zlib
w,h=512,256
pixels=bytearray()
for y in range(h):
    pixels.append(0)
    v=(y+.5)/h
    for x in range(w):
        u=(x+.5)/w
        ceiling=max(0,math.cos(v*math.pi))
        base=[.12+.22*ceiling,.10+.21*ceiling,.09+.23*ceiling]
        for cx,cy,sx,sy,color in [(.23,.29,.075,.12,(.75,.67,.53)),(.72,.38,.035,.20,(.45,.56,.72))]:
            dx=min(abs(u-cx),1-abs(u-cx))/sx
            dy=(v-cy)/sy
            light=math.exp(-2*(dx**4+dy**4))
            base=[a+light*b for a,b in zip(base,color)]
        pixels.extend(round(255*min(1,max(0,c))) for c in base)
def chunk(kind,data):return struct.pack('>I',len(data))+kind+data+struct.pack('>I',zlib.crc32(kind+data)&0xffffffff)
png=b'\x89PNG\r\n\x1a\n'+chunk(b'IHDR',struct.pack('>IIBBBBB',w,h,8,2,0,0,0))+chunk(b'sRGB',b'\0')+chunk(b'IDAT',zlib.compress(bytes(pixels),9))+chunk(b'IEND',b'')
output=Path(__file__).resolve().parents[1]/'realism/studio-environment.png'
output.parent.mkdir(parents=True,exist_ok=True);output.write_bytes(png)
print(output)
