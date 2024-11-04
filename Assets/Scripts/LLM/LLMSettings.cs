/// <summary>
/// Represents the configuration settings for the Language Learning Model (LLM)
/// </summary>
[System.Serializable]
public class LLMSettings
{
    public string apiToken;
    public string apiUrl;
    public string model;
    
    // Temperature settings
    public float temp;
    public bool temperature_last;
    
    // Sampling parameters
    public float top_p;
    public int top_k;
    public float top_a;
    public float tfs;
    public float epsilon_cutoff;
    public float eta_cutoff;
    public float typical_p;
    public float min_p;
    
    // Repetition penalty settings
    public float rep_pen;
    public int rep_pen_range;
    public float rep_pen_decay;
    public float rep_pen_slope;
    
    // Additional generation parameters
    public int no_repeat_ngram_size;
    public float penalty_alpha;
    public int num_beams;
    public float length_penalty;
    public int min_length;
    public float encoder_rep_pen;
    public float freq_pen;
    public float presence_pen;
    
    // Control flags
    public float skew;
    public bool do_sample;
    public bool early_stopping;
    public bool dynatemp;
    public float min_temp;
    public float max_temp;
    public float dynatemp_exponent;
    
    // Token handling
    public bool add_bos_token;
    public bool ban_eos_token;
    public bool skip_special_tokens;
    
    // Mirostat settings
    public int mirostat_mode;
    public float mirostat_tau;
    public float mirostat_eta;
    
    // Generation control
    public float guidance_scale;
    public string negative_prompt;
    public string grammar_string;
    public object json_schema;
    public string banned_tokens;
    
    public string[] dry_sequence_breakers;
    public int genamt;
    public int max_length;
}
