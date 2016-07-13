#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>

#include <sphinxbase/err.h>
#include <sphinxbase/ad.h>
#include <pocketsphinx.h>

#define HOTWORD_KEY "hotwordsearch"
#define LM_KEY "lmsearch"

static const arg_t args_def[] = {
	POCKETSPHINX_OPTIONS,
	CMDLN_EMPTY_OPTION
};

static void
sleep_msec (uint msec)
{
	usleep (msec * 1000);
}

static char const *
acquire_from_mic (ps_decoder_t *ps, ad_rec_t *ad, int need_final)
{
    int16 adbuf[4096];
    uint8 utt_started, in_speech;
    int32 k;
    char const *hyp;

    if (ps_start_utt(ps) < 0) {
        E_ERROR ("Failed to start utterance\n");
        return NULL;
    }
    utt_started = FALSE;
    E_INFO("Ready....\n");

    for (;;) {
        if ((k = ad_read(ad, adbuf, 4096)) < 0)
            E_FATAL("Failed to read audio\n");
        ps_process_raw(ps, adbuf, k, FALSE, FALSE);
        in_speech = ps_get_in_speech(ps);
        if (in_speech && !utt_started) {
            utt_started = TRUE;
            E_INFO("Listening...\n");
        }
        if (!in_speech && utt_started) {
            /* speech -> silence transition, time to start new utterance  */
            ps_end_utt(ps);
            
            hyp = ps_get_hyp (ps, NULL);
            if (hyp != NULL)
	            return hyp;

            if (ps_start_utt(ps) < 0) {
                E_ERROR("Failed to start utterance\n");
                return NULL;
            }
            utt_started = FALSE;
            E_INFO("Ready....\n");
        }
        sleep_msec(100);
    }

    return NULL;
}

ps_decoder_t*
init_sphinx (const char *config_file, const char *hotword)
{
	cmd_ln_t *config = cmd_ln_parse_file_r (NULL, args_def, config_file, 1);
	ps_default_search_args (config);
	ps_decoder_t *ps = ps_init (config);

	ps_set_lm_file (ps, LM_KEY, "6641.lm");
	ps_set_keyphrase (ps, HOTWORD_KEY, hotword);
	
	return ps;
}

void free_sphinx (ps_decoder_t *ps, ad_rec_t *ad)
{
	if (ad != NULL)
		ad_close (ad);
	if (ps != NULL)
		ps_free (ps);
}

ad_rec_t*
open_recording_device (ps_decoder_t *ps, const char *device_name)
{
	ad_rec_t *ad;
	int samprate = (int) cmd_ln_float32_r (ps_get_config (ps), "-samprate");
	if ((ad = ad_open_dev (device_name, samprate)) == NULL) {
		E_ERROR("Failed to open audio device\n");
		return NULL;
	}
	if (ad_start_rec(ad) < 0) {
        E_ERROR("Failed to start recording\n");
        return NULL;
	}
	return ad;
}

int
wait_for_hotword (ps_decoder_t *ps, ad_rec_t *ad)
{
	if (ps_set_search (ps, HOTWORD_KEY) < 0) {
		E_ERROR("Couldn't set hotwordsearch\n");
		return 0;
	}

	const char *keyphrase = ps_get_kws (ps, HOTWORD_KEY);

	const char *hyp = NULL;

	do {
		hyp = acquire_from_mic (ps, ad, FALSE);
		puts (hyp);
	} while (hyp != NULL && strcmp (keyphrase, hyp) != 0);

	return hyp == NULL ? 0 : 1;
}

const char*
wait_for_command (ps_decoder_t *ps, ad_rec_t *ad)
{
	if (ps_set_search (ps, LM_KEY) < 0) {
		E_ERROR("Couldn't set command search\n");
		return NULL;
	}

	return strdup(acquire_from_mic (ps, ad, TRUE));
}

#ifdef HAS_MAIN
int main (int argc, const char *argv[])
{
	ps_decoder_t *ps = init_sphinx (argv[0], argv[1]);
	ad_rec_t ad = open_recording_device (argv[2]);

	printf ("Got hotword? %d\n", wait_for_hotword (ps, ad));

	free_sphinx (ps, ad);
}
#endif
