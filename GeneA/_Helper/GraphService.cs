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
        }

        private readonly IRepository<Person> _repository;

        public async Task<Graph> ToGraphAsync(Person person)
        {
            return await Task.Run(() =>
              {

                  var graph = new Graph();

                  if (person.Father != null && person.Mother != null)
                  {
                      //TODO: see how make full functional trees with this

                      person.Father = _repository.FindById(person.Father.Id);
                      person.Mother = _repository.FindById(person.Mother.Id);

                      var edge = new Edge(person.Father, person, 
                          tailSymbol: Edge.Symbol.None, headSymbol: Edge.Symbol.None);

                      graph.Edges.Add(edge);
                  }

                  return graph;
              });
        }
    }
}
