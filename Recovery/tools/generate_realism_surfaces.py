"""Original procedural microstructure maps, not height inferred from baked photos.
Offline only: no texture generation or allocations in gameplay collision callbacks.
"""
from pathlib import Path
import math, struct, zlib
out=Path(__file__).resolve().parents[1]/'realism'
w=h=512
maps={name:bytearray() for name in ['wood-normal','wood-roughness','metal-roughness']}
for y in range(h):
 for buf in maps.values():buf.append(0)
 v=y/h
 for x in range(w):
  u=x/w
  phase=2*math.pi*(67*v+.25*math.sin(2*math.pi*3*u))
  nx=.008*math.cos(2*math.pi*3*u)*math.cos(phase)
  ny=.065*math.cos(phase)+.018*math.cos(2*math.pi*151*v)
  nz=1;length=math.sqrt(nx*nx+ny*ny+nz*nz)
  maps['wood-normal'].extend(round(255*(.5+.5*n/length)) for n in [nx,ny,nz])
  grain=.5+.5*math.sin(phase)
  rough=round(255*(.55+.10*grain));maps['wood-roughness'].extend([rough]*3)
  variation=math.sin(2*math.pi*(113*u+7*v))*math.sin(2*math.pi*(31*v))
  rough=round(255*(.36+.025*variation));maps['metal-roughness'].extend([rough]*3)
def chunk(k,d):return struct.pack('>I',len(d))+k+d+struct.pack('>I',zlib.crc32(k+d)&0xffffffff)
for name,data in maps.items():
 # No sRGB tag: these are material data, not color photographs.
 png=b'\x89PNG\r\n\x1a\n'+chunk(b'IHDR',struct.pack('>IIBBBBB',w,h,8,2,0,0,0))+chunk(b'IDAT',zlib.compress(bytes(data),9))+chunk(b'IEND',b'')
 (out/(name+'.png')).write_bytes(png)
print('Generated three 512 x 512 surface maps')
