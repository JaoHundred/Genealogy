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

            _graph  = new Graph();
        }

        private readonly IRepository<Person> _repository;

        private Graph _graph;

        public async Task<Graph> ToGraphAsync(Person person)
        {
            return await Task.Run(async() =>
              {
                  if (person.Father != null && person.Mother != null)
                  {
                      //TODO: try to customize the graph control spacing and customize the text spacing

                      person.Father = _repository.FindById(person.Father.Id);
                      person.Mother = _repository.FindById(person.Mother.Id);

                      var fatherEdge = new Edge(person.Father, person, 
                          tailSymbol: Edge.Symbol.None, headSymbol: Edge.Symbol.None);

                      var motherEdge = new Edge(person.Mother, person,
                          tailSymbol: Edge.Symbol.None, headSymbol: Edge.Symbol.None);

                      await ToGraphAsync(person.Father);
                      await ToGraphAsync(person.Mother);

                      _graph.Edges.Add(fatherEdge);
                      _graph.Edges.Add(motherEdge);
                  }

                  return _graph;
              });
        }
    }
}
