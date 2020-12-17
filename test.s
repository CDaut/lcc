.globl main
main:
movl $3, %eax
push %rax
movl $2, %eax
pop %rcx
sub %ecx, %eax
push %rax
movl $1, %eax
pop %rcx
sub %ecx, %eax
ret
