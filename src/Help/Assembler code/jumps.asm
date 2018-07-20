            org 40000
            xor a
            out (#fe), a
            jr  $-2 ; jump back 2 bytes(here jump to out(#FE),a instruction
            ret