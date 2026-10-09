# Generates the 24 gem sprites: python tools/generate_gem_sprites.py Assets/OuterSources/BlockSprites
import math,sys
from PIL import Image,ImageDraw,ImageFilter
S=4;N=256*S
COL={'Blue':(52,152,235),'Green':(76,200,90),'Pink':(244,114,182),'Purple':(150,95,220),'Red':(232,64,64),'Yellow':(250,200,50)}
def shade(c,f): return tuple(max(0,min(255,int(v*f))) for v in c)
def rr(d,box,r,fill): d.rounded_rectangle(box,r,fill=fill)
def star(cx,cy,R,r,n=5):
    pts=[]
    for i in range(n*2):
        a=-math.pi/2+i*math.pi/n; k=R if i%2==0 else r
        pts.append((cx+k*math.cos(a),cy+k*math.sin(a)))
    return pts
def make(c,tier):
    im=Image.new('RGBA',(N,N),(0,0,0,0))
    m=14*S; box=(m,m,N-m,N-m); rad=58*S
    # shadow
    sh=Image.new('RGBA',(N,N),(0,0,0,0)); rr(ImageDraw.Draw(sh),(m,m+8*S,N-m,N-m+8*S),rad,(0,0,0,90))
    im=Image.alpha_composite(im,sh.filter(ImageFilter.GaussianBlur(6*S)))
    mask=Image.new('L',(N,N),0); rr(ImageDraw.Draw(mask),box,rad,255)
    # vertical gradient body
    body=Image.new('RGBA',(N,N))
    px=body.load()
    for y in range(N):
        t=(y-m)/(N-2*m); t=max(0,min(1,t))
        f=1.25-0.55*t
        for x in range(N): px[x,y]=shade(c,f)+(255,)
    body.putalpha(mask)
    im=Image.alpha_composite(im,body)
    # inner border darker
    ring=Image.new('RGBA',(N,N),(0,0,0,0)); d=ImageDraw.Draw(ring)
    d.rounded_rectangle(box,rad,outline=shade(c,0.55)+(255,),width=5*S)
    im=Image.alpha_composite(im,ring)
    # gloss
    g=Image.new('RGBA',(N,N),(0,0,0,0)); gd=ImageDraw.Draw(g)
    rr(gd,(m+16*S,m+12*S,N-m-16*S,m+88*S),40*S,(255,255,255,85))
    g.putalpha(Image.composite(g.split()[3],Image.new('L',(N,N),0),mask))
    im=Image.alpha_composite(im,g.filter(ImageFilter.GaussianBlur(2*S)))
    # emblem
    cx=N//2; cy=N//2+10*S
    e=Image.new('RGBA',(N,N),(0,0,0,0)); ed=ImageDraw.Draw(e)
    fill=(255,255,255,235); dark=shade(c,0.45)+(255,)
    def emb(draw,off,col):
        ox,oy=off
        if tier==1: draw.ellipse((cx-22*S+ox,cy-22*S+oy,cx+22*S+ox,cy+22*S+oy),fill=col)
        elif tier==2: draw.polygon([(cx+ox,cy-34*S+oy),(cx+30*S+ox,cy+oy),(cx+ox,cy+34*S+oy),(cx-30*S+ox,cy+oy)],fill=col)
        elif tier==3: draw.polygon([(x+ox,y+oy) for x,y in star(cx,cy,42*S,18*S)],fill=col)
    if tier:
        emb(ed,(0,3*S),dark); emb(ed,(0,0),fill)
    im=Image.alpha_composite(im,e)
    return im.resize((256,256),Image.LANCZOS)
out=sys.argv[1]
for n,c in COL.items():
    for t,s in enumerate(['Default','A','B','C']):
        make(c,t).save(f'{out}/{n}_{s}.png')
