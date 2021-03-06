#pragma once
#include <unistd.h>
#include <sys/syscall.h>
#include <stdio.h>
#include <errno.h>

#define sys_gettid() syscall(SYS_gettid)

#define pr_info(fmt, ...)				\
	fprintf( stderr, "%8d: " fmt, sys_gettid(), ##__VA_ARGS__)

#define pr_err(fmt, ...)				\
	fprintf( stderr, "%8d: Error (%s:%d): " fmt, sys_gettid(),\
		       __FILE__, __LINE__, ##__VA_ARGS__)
	
#define pr_warn(fmt, ...)				\
	fprintf( stderr, "%8d: Warning (%s:%d): " fmt, sys_gettid(),\
		       __FILE__, __LINE__, ##__VA_ARGS__)

#define pr_perror(fmt, ...)				\
	pr_err(fmt ": %m\n", ##__VA_ARGS__)

#define pr_msg(fmt, ...)				\
	fprintf( stderr, fmt, ##__VA_ARGS__)


#define pr_trace(fmt, ...)				\
	fprintf( stderr, "%8d: %s: " fmt, sys_gettid(), __func__,	\
		##__VA_ARGS__)