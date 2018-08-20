#pragma once

#ifdef SIDLIB_EXPORTS
#define SIDLIB_API __declspec(dllexport)
#else
#define SIDLIB_API __declspec(dllimport)
#endif

#define CALL_API __stdcall