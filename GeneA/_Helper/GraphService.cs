using AvaloniaGraphControl;
using Model.Interfaces;
using ModelA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Helper
{
    public class GraphService
    {
        public GraphService(IRepository<Person> repository)
        {
            _repository = repository;

            _graph = new Graph();
        }

        private readonly IRepository<Person> _repository;

        private Graph _graph;
        private List<Edge> _edges = new List<Edge>();

        public async Task<Graph> ToGraphAsync(Person person)
        {
            return await Task.Run(async () =>
              {
                  if (person.Father != null && person.Mother != null)
                  {
                      person.Father = _repository.FindById(person.Father.Id);
                      person.Mother = _repository.FindById(person.Mother.Id);

                      var fatherEdge = new Edge(person.Father, person);
                      var motherEdge = new Edge(person.Mother, person);

                      var spouse = new Edge(person.Father, person.Mother, headSymbol: Edge.Symbol.Arrow, tailSymbol: Edge.Symbol.Arrow
                          ,label: DynamicTranslate.Translate(MessageConsts.Spouse));

                      _graph.Edges.Add(spouse);

                      _graph.Edges.Add(fatherEdge);
                      _graph.Edges.Add(motherEdge);

                      await ToGraphAsync(person.Father);
                      await ToGraphAsync(person.Mother);
                  }

                  //TODO: sometimes graph changes its order, see why this is happening

                  return _graph;
              });
        }
    }
}
