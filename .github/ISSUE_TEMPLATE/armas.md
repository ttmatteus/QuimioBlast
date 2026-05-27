---
name: 5. Nova Funcionalidade - Armas e Poderes
about: Sugira uma melhoria ou nova feature para o projeto
title: "[FEATURE] Sistema de Uso de Armas e Poderes Ativos"
labels: enhancement
assignees: ''
---

### 📋 Resumo da Ideia
<!-- Descreva em uma frase o que você gostaria de ver no projeto -->
Implementar o sistema base de combate que permita ao jogador equipar, alternar e utilizar diferentes armas e poderes ativos durante a gameplay.

### ⚠️ Problema que Resolve
<!-- Qual dor ou limitação essa feature endereça? -->
Atualmente, o jogador não possui formas de interagir ofensivamente ou defensivamente com o cenário e inimigos, limitando a experiência de jogo a apenas movimentação e exploração básica. 

### 💡 Solução Proposta
<!-- Descreva como você imagina que essa funcionalidade deveria funcionar -->
Criar uma estrutura modular (ou um gerenciador de combate) onde:
* O player possa ter um botão para ataque básico utilizando a arma equipada.
* O player possa disparar um poder/habilidade ativo (ex: magia, dash, ou escudo) associado a um botão de atalho e que possua um tempo de recarga (*cooldown*).
* O sistema deve aceitar a configuração de diferentes tipos de armas e poderes (via Scriptable Objects ou classes base) para facilitar a criação de novos conteúdos futuramente.

### 🔄 Alternativas Consideradas
<!-- Existe outra forma de resolver o problema? Por que você prefere a solução proposta? -->
Consideramos criar scripts separados e rígidos para cada arma/poder diretamente no script do Player. Preferimos a solução modular e genérica porque ela evita código duplicado e permite que novos itens e habilidades sejam adicionados por designers sem a necessidade de reescrever a lógica central do personagem.

### ✅ Critérios de Aceitação
<!-- Como saberemos que essa feature foi implementada corretamente? Liste os critérios -->
- [ ] O jogador consegue equipar uma arma e o ataque é executado ao pressionar o botão de ação.
- [ ] O jogador consegue ativar um poder e o tempo de recarga (*cooldown*) impede o uso contínuo desordenado.
- [ ] O sistema detecta corretamente as colisões ou áreas de efeito do ataque/poder nos alvos/inimigos.

### 🎨 Mockups / Referências
<!-- Se aplicável, adicione esboços, prints de referência ou links de inspiração -->
* *Referência visual de HUD para cooldowns: Barra de habilidades clássica de jogos de RPG/Ação com indicador visual de recarga.*

### ℹ️ Contexto Adicional
<!-- Alguma outra informação que ajude a entender a solicitação? -->
Integrar as animações básicas de ataque e os efeitos visuais (VFX) simples junto à execução dos comandos para dar o feedback visual necessário ao jogador.
