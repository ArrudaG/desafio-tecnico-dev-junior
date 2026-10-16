import React, { useState } from 'react';
import axios from 'axios';
import { api } from './services/api';
import type {BuscaRequest, BuscaResponse} from './types/busca';
import './App.css';

const DIAS_OPCOES = [
  { label: 'Segunda', value: 'segunda' },
  { label: 'Terça', value: 'terca' },
  { label: 'Quarta', value: 'quarta' },
  { label: 'Quinta', value: 'quinta' },
  { label: 'Sexta', value: 'sexta' },
  { label: 'Sábado', value: 'sabado' },
  { label: 'Domingo', value: 'domingo' },
];

export function App() {
  // Estado do formulário
  const [idioma, setIdioma] = useState('Espanhol');
  const [dias, setDias] = useState<string[]>(['segunda']);
  const [inicioFaixa, setInicioFaixa] = useState('19:00');
  const [fimFaixa, setFimFaixa] = useState('20:00');
  const [duracaoMinutos, setDuracaoMinutos] = useState<number>(60);

  // Estados da interface (Requisito 5 da avaliação)
  const [loading, setLoading] = useState(false);
  const [resultado, setResultado] = useState<BuscaResponse | null>(null);
  const [errosValidacao, setErrosValidacao] = useState<string[]>([]);
  const [erroConexao, setErroConexao] = useState<string | null>(null);

  const handleDiaChange = (diaValue: string) => {
    setDias((prev) =>
        prev.includes(diaValue)
            ? prev.filter((d) => d !== diaValue)
            : [...prev, diaValue]
    );
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Resetar estados anteriores
    setLoading(true);
    setResultado(null);
    setErrosValidacao([]);
    setErroConexao(null);

    const payload: BuscaRequest = {
      idioma,
      dias,
      inicioFaixa,
      fimFaixa,
      duracaoMinutos: Number(duracaoMinutos),
    };

    try {
      const response = await api.post<BuscaResponse>('/professores/busca', payload);
      setResultado(response.data);
    } catch (err: unknown) {
      if (axios.isAxiosError(err)) {
        if (err.response && err.response.status === 400) {
          // Trata erros de validação retornados pela API C#
          const data = err.response.data;
          if (data.errors) {
            const mensagens = Object.values(data.errors).flat() as string[];
            setErrosValidacao(mensagens);
          } else if (typeof data === 'string') {
            setErrosValidacao([data]);
          } else {
            setErrosValidacao(['Dados informados inválidos. Verifique os campos.']);
          }
        } else {
          // Falha de comunicação ou erro 500
          setErroConexao('Não foi possível conectar à API. Verifique se o servidor backend está rodando.');
        }
      } else {
        setErroConexao('Ocorreu um erro inesperado.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
      <div className="container">
        <h1>Encontre um Professor</h1>

        {/* Formulario */}
        <form onSubmit={handleSubmit} className="form-card">
          <div className="form-group">
            <label htmlFor="idioma">Idioma:</label>
            <select
                id="idioma"
                value={idioma}
                onChange={(e) => setIdioma(e.target.value)}
            >
              <option value="Inglês">Inglês</option>
              <option value="Espanhol">Espanhol</option>
              <option value="Francês">Francês</option>
            </select>
          </div>

          <div className="form-group">
            <label>Dias da Semana:</label>
            <div className="checkbox-group">
              {DIAS_OPCOES.map((d) => (
                  <label key={d.value} className="checkbox-label">
                    <input
                        type="checkbox"
                        checked={dias.includes(d.value)}
                        onChange={() => handleDiaChange(d.value)}
                    />
                    {d.label}
                  </label>
              ))}
            </div>
          </div>
          
          <div className="form-group">
            <label>Insira as informações referentes a sua faixa de horário disponível:</label>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="inicio">Início da Faixa:</label>
              <input
                  id="inicio"
                  type="time"
                  value={inicioFaixa}
                  onChange={(e) => setInicioFaixa(e.target.value)}
              />
            </div>

            <div className="form-group">
              <label htmlFor="fim">Fim da Faixa:</label>
              <input
                  id="fim"
                  type="time"
                  value={fimFaixa}
                  onChange={(e) => setFimFaixa(e.target.value)}
              />
            </div>

            <div className="form-group">
              <label htmlFor="duracao">Duração:</label>
              <select
                  id="duracao"
                  value={duracaoMinutos}
                  onChange={(e) => setDuracaoMinutos(Number(e.target.value))}
              >
                <option value={30}>30 minutos</option>
                <option value={60}>60 minutos</option>
                <option value={90}>90 minutos</option>
              </select>
            </div>
          </div>

          <button type="submit" disabled={loading} className="btn-submit">
            {loading ? 'Buscando...' : 'Buscar Professores'}
          </button>
        </form>

        {/* ESTADO 1: Mensagens de Erro de Validação (HTTP 400) */}
        {errosValidacao.length > 0 && (
            <div className="alert alert-danger">
              <h4>Erros de Validação:</h4>
              <ul>
                {errosValidacao.map((msg, idx) => (
                    <li key={idx}>{msg}</li>
                ))}
              </ul>
            </div>
        )}

        {/* ESTADO 2: Falha de Comunicação */}
        {erroConexao && (
            <div className="alert alert-danger">
              <p>{erroConexao}</p>
            </div>
        )}

        {/* ESTADO 3: Carregando */}
        {loading && (
            <div className="loading-spinner">
              <p>Buscando professores disponíveis...</p>
            </div>
        )}

        {/* Exibição dos Resultados */}
        {resultado && !loading && (
            <div className="resultados-container">
              <h2>Professores Encontrados</h2>

              {/* ESTADO 4: Nenhum professor encontrado */}
              {resultado.professores.length === 0 ? (
                  <div className="alert alert-info">
                    <p>Nenhum professor encontrado para os critérios informados.</p>
                  </div>
              ) : (
                  <div className="cards-grid">
                    {resultado.professores.map((prof) => (
                        <div key={prof.id} className="card-professor">
                          <h3>{prof.nome}</h3>
                          <p className="subtext">
                            Aulas marcadas na semana: <strong>{prof.aulasNaSemana}</strong>
                          </p>
                          <div className="horarios-list">
                            <strong>Horários disponíveis:</strong>
                            <div className="tags">
                              {prof.horarios.map((h, i) => (
                                  <span key={i} className="tag-horario">
                          {h}
                        </span>
                              ))}
                            </div>
                          </div>
                        </div>
                    ))}
                  </div>
              )}

              {/* Diferencial: Lista de Não Elegíveis */}
              {resultado.naoElegiveis.length > 0 && (
                  <div className="nao-elegiveis-section">
                    <h3>Professores Não Elegíveis</h3>
                    <table className="tabla-nao-elegiveis">
                      <thead>
                      <tr>
                        <th>Nome</th>
                        <th>Motivo</th>
                      </tr>
                      </thead>
                      <tbody>
                      {resultado.naoElegiveis.map((prof) => (
                          <tr key={prof.id}>
                            <td>{prof.nome}</td>
                            <td>
                              <span className="badge-motivo">{prof.motivo}</span>
                            </td>
                          </tr>
                      ))}
                      </tbody>
                    </table>
                  </div>
              )}
            </div>
        )}
      </div>
  );
}

export default App;