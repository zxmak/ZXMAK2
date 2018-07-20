label1 DEFL #1234   ; define label(label1=#1234)

            org 40000            
            ld bc, label1
            ld a,  XPOS
            ret         

DEFM   "some text"  ; define string
DEFS   2, " "       ; defines 2 spaces into memory
DEFW   16384        ; define word(2bytes)
DEFB   #77          ; define byte
XPOS   EQU #44      ; as DEFB/DEFW, but not compiled into code