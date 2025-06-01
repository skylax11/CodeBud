from flask import Flask, request, jsonify
from transformers import AutoTokenizer, AutoModelForSeq2SeqLM, pipeline
from sentence_transformers import SentenceTransformer
import numpy as np
import faiss

app = Flask(__name__)

# 🔧 Model yükle (RAM dostu)
model_name = "google/flan-t5-small"
tokenizer = AutoTokenizer.from_pretrained(model_name)
model = AutoModelForSeq2SeqLM.from_pretrained(model_name)
pipe = pipeline("text2text-generation", model=model, tokenizer=tokenizer, device=-1)
print("✅ FLAN-T5 model ready.")

# 📚 RAG verisi (lightweight context)
context_docs = [
    "CodeBud is a developer Q&A platform created for a web programming course.",
    "It allows users to ask coding questions and get answers using AI.",
    "The site is built using ASP.NET MVC and Entity Framework.",
    "The founders of CodeBud are Devran Yilmaz, Eyüp Bag, Mehmet Eren Reyhanlioglu, Furkan Benze."
]

embedder = SentenceTransformer("all-MiniLM-L6-v2")
embeddings = embedder.encode(context_docs)
index = faiss.IndexFlatL2(embeddings.shape[1])
index.add(np.array(embeddings))

@app.route("/ask", methods=["POST"])
def ask():
    try:
        user_question = request.json.get("question", "") if request.json else ""
        if not user_question.strip():
            return jsonify({"error": "Empty question"}), 400

        # 🔍 En yakın context'i bul
        q_emb = embedder.encode([user_question])
        _, I = index.search(np.array(q_emb), k=1)
        top_context = context_docs[I[0][0]]

        # 🧠 Prompt oluştur
        prompt = f"Context: {top_context}\nQuestion: {user_question}\nAnswer:"
        print("🧠 Prompt:", prompt)

        # 🤖 Cevap üret
        output = pipe(prompt, max_new_tokens=100)
        result_text = output[0].get("generated_text", "[no output]")

        return jsonify({"answer": result_text})

    except Exception as e:
        print("❌ Error:", str(e))
        return jsonify({"error": str(e)}), 500

if __name__ == "__main__":
    app.run(port=5001)
