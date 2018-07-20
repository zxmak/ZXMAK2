macroBorderRandom MACRO var1
ld a, var1: out(#fe),a
ENDM
macroAddressValue MACRO var1
ld a, var1: out(#fe),a
ENDM

org 40000
macroBorderRandom 1 ; use macro
macroAddressValue(#1234)
ret
