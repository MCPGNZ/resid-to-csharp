#pragma once
#include "dll_defines.h"
#include "resid-0.16/sid.h"

extern "C"
{
    namespace pk
    {
        /* lifetime */
        SIDLIB_API void* CALL_API create_sid();
        SIDLIB_API void  CALL_API release_sid(SID* sid);

        /* features */
        SIDLIB_API void CALL_API set_chip_model(SID* sid, chip_model model);
        SIDLIB_API void CALL_API enable_filter(SID* sid, bool enable);
        SIDLIB_API void CALL_API enable_external_filter(SID* sid, bool enable);

        /* sampling */
        SIDLIB_API bool CALL_API set_sampling_parameters(SID* sid, double clock_freq, sampling_method method,
            double sample_freq, double pass_freq = -1, double filter_scale = 0.97);
        SIDLIB_API void CALL_API adjust_sampling_frequency(SID* sid, double sample_freq);

        /* simulation */
        SIDLIB_API void CALL_API clock(SID* sid);
        SIDLIB_API void CALL_API clock_t(SID* sid, cycle_count delta_t);
        SIDLIB_API int  CALL_API clock_tb(SID* sid, cycle_count& delta_t, short* buf, int n, int interleave = 1);
        SIDLIB_API void CALL_API reset(SID* sid);

        /* state */
        SIDLIB_API reg8 CALL_API read(SID* sid, reg8 offset);
        SIDLIB_API void CALL_API write(SID* sid, reg8 offset, reg8 value);
        SIDLIB_API void CALL_API input(SID* sid, int sample);
        SIDLIB_API int  CALL_API output(SID* sid);
        SIDLIB_API int  CALL_API output_n(SID* sid, int bits);

        /* internal */
        SIDLIB_API void CALL_API read_state(SID* sid, void* bytes);
        SIDLIB_API void CALL_API write_state(SID* sid, void* bytes);
    }
}