#include "sidlib.h"
#include <cstring>

extern "C"
{
    namespace pk
    {
        /* lifetime */
        void* CALL_API create_sid()
        {
            return new SID{};
        }
        void  CALL_API release_sid(SID* sid)
        {
            delete sid;
        }

        /* features */
        void CALL_API set_chip_model(SID* sid, const chip_model model)
        {
            sid->set_chip_model(model);
        }
        void CALL_API enable_filter(SID* sid, const bool enable)
        {
            sid->enable_filter(enable);
        }
        void CALL_API enable_external_filter(SID* sid, const bool enable)
        {
            sid->enable_external_filter(enable);
        }

        /* sampling */
        bool CALL_API set_sampling_parameters(SID* sid, const double clock_freq, const sampling_method method,
            const double sample_freq, const double pass_freq /* = -1 */, const double filter_scale /* = 0.97 */)
        {
            return sid->set_sampling_parameters(clock_freq, method, sample_freq, pass_freq, filter_scale);
        }
        void CALL_API adjust_sampling_frequency(SID* sid, const double sample_freq)
        {
            sid->adjust_sampling_frequency(sample_freq);
        }

        /* simulation */
        void CALL_API clock(SID* sid)
        {
            sid->clock();
        }
        void CALL_API clock_t(SID* sid, const cycle_count delta_t)
        {
            sid->clock(delta_t);
        }
        int  CALL_API clock_tb(SID* sid, cycle_count& delta_t, short* buf, int n, const int interleave /* = 1 */)
        {
            return sid->clock(delta_t, buf, interleave);
        }
        void CALL_API reset(SID* sid)
        {
            sid->reset();
        }

        /* state */
        reg8 CALL_API read(SID* sid, const reg8 offset)
        {
            return sid->read(offset);
        }
        void CALL_API write(SID* sid, const reg8 offset, const reg8 value)
        {
            sid->write(offset, value);
        }
        void CALL_API input(SID* sid, const int sample)
        {
            sid->input(sample);
        }
        int  CALL_API output(SID* sid)
        {
            return sid->output();
        }
        int  CALL_API output_n(SID* sid, const int bits)
        {
            return sid->output(bits);
        }

        /* internal */
        void CALL_API read_state(SID* sid, void* bytes)
        {
            const auto state = sid->read_state();
            memcpy(bytes, state.sid_register, 32);
        }
        void CALL_API write_state(SID* sid, void* bytes)
        {
            auto ref = static_cast<SID*>(sid);
            auto state = ref->read_state();
            memcpy(reinterpret_cast<void*>(state.sid_register), bytes, 32);
            ref->write_state(state);
        }
    }
}