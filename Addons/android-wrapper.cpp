#include "llama.h"
#include <vector>
#include <string>
#include <cstring>

extern "C" {

static llama_model* model = nullptr;
static llama_context* ctx = nullptr;
static const llama_vocab* vocab = nullptr;

int llama_android_init(const char* model_path) {
    llama_backend_init();

    llama_model_params mparams = llama_model_default_params();
    model = llama_model_load_from_file(model_path, mparams);
    if (!model) return -1;

    llama_context_params cparams = llama_context_default_params();
    cparams.n_ctx = 8192;

    ctx = llama_init_from_model(model, cparams);
    if (!ctx) return -2;

    vocab = llama_model_get_vocab(model);
    return 0;
}

int llama_android_infer(const char* prompt, char* output, int max_len) {
    if (!ctx) return -1;

    std::vector<llama_token> tokens(prompt ? strlen(prompt) * 3 : 1);

    int n_tokens = llama_tokenize(vocab, prompt, strlen(prompt), tokens.data(), tokens.size(), true, false);
    if (n_tokens < 0) return -2;
    tokens.resize(n_tokens);

    llama_batch batch = llama_batch_get_one(tokens.data(), n_tokens);
    if (llama_decode(ctx, batch) != 0) return -3;

    // greedy sampling
    llama_sampler* sampler = llama_sampler_init_greedy();

    std::string result;
    for (int i = 0; i < 256; i++) {
        llama_token t = llama_sampler_sample(sampler, ctx, -1);
        if (t == llama_vocab_eos(vocab)) break;

        char piece[32];
        int n = llama_token_to_piece(vocab, t, piece, sizeof(piece), 0, true);
        if (n > 0) result.append(piece, n);

        llama_batch next_batch = llama_batch_get_one(&t, 1);
        if (llama_decode(ctx, next_batch) != 0) break;
    }

    llama_sampler_free(sampler);

    strncpy(output, result.c_str(), max_len);
    output[max_len - 1] = '\0';
    return 0;
}

void llama_android_free() {
    if (ctx) llama_free(ctx);
    if (model) llama_model_free(model);
    llama_backend_free();
}

}